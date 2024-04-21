using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ChatBotService.Prompts;
using Shared.Models;
using Shared.Enums;
using Shared.Contracts;

namespace ChatBotService.Services
{
    public class ChatCompletion : IChatCompletion
    {
        private readonly ITokenManager _tokenManager;
        private readonly ILogger<ChatCompletion> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _endpoint;
        private readonly string _key;
        private readonly string _deploymentOrModelName4;
        private readonly string _deploymentOrModelName35;

        public ChatCompletion(IConfiguration configuration, ITokenManager tokenManager)
        {
            _configuration = configuration;
            _tokenManager = tokenManager;

            // Get the Azure OpenAI Service configuration values
            _endpoint = _configuration["AzureOpenAIServiceOptions_Endpoint"] ?? throw new ArgumentException("AzureOpenAIServiceOptions_Endpoint is Missing");
            _deploymentOrModelName4 = _configuration["AzureOpenAIServiceOptions_DeploymentOrModelName"] ?? throw new ArgumentException("AzureOpenAIServiceOptions_DeploymentOrModelName is Missing");
            _deploymentOrModelName35 = _configuration["AzureOpenAIServiceOptions_DeploymentOrModelName35"] ?? throw new ArgumentException("AzureOpenAIServiceOptions_DeploymentOrModelName35 is Missing");
            _key = _configuration["AzureOpenAIServiceOptions_Key"] ?? throw new ArgumentException("AzureOpenAIServiceOptions_Key is Missing"); ;
        }

        //public async Task<(ChatCompletions response, ChatCompletions followup, int promptTokens, int completionTokens, int suggestionTokens)> ChatCompletionAsync(EZCompletionOptions ezCompletionOptions)
        public async Task<Completion> ChatCompletionAsync(CompletionOverrides ezCompletionOptions)
        {
            try
            {
                OpenAIClient client = new(
                    new Uri(_endpoint),
                    new AzureKeyCredential(_key));

                // Extract ChatMessage objects from the EZCompletionOptions object
                var chatMessages = ezCompletionOptions.ChatMessages;

                // Build system prompt from template
                var systemPrompt = new ChatRequestSystemMessage(PromptTemplates.SystemPromptTemplate);

                // Add system prompt to chat history to beginning of list
                //chatMessages.Add(new ChatMessage(Role.System, PromptTemplates.SystemPromptTemplate, 0));

                // Main prompt is the current text request from the user. It will always be the last message in the chat history
                var mainPrompt = (chatMessages.LastOrDefault());

                // Remove the last message from the chat history as it will be the main prompt.
                // We will add it back after the chat history is optimized
                chatMessages.RemoveAt(chatMessages.Count - 1);
                
                // At this point, the chat history is not optimized
                // Send the chat history to the token manager to optimize so that we don't exceed the max tokens
                if (chatMessages.Count > 1)
                {
                    chatMessages = await _tokenManager.OptimizeChatHistoryAsync(chatMessages);
                }

                // Chat history is now optimized
                // *** Build the completion options object ***
                var completionOptions = new ChatCompletionsOptions();

                // Add the system prompt first
                completionOptions.Messages.Add(systemPrompt);

                // Add the chat history
                // Skip first message which will be the current prommpt
                foreach (var message in ezCompletionOptions.ChatMessages.Take(ezCompletionOptions.ChatMessages.Count))
                {
                    if (message.Role == Role.Assistant)
                    {
                        completionOptions.Messages.Add(new ChatRequestAssistantMessage(message.Content));
                    }
                    else
                    {
                        completionOptions.Messages.Add(new ChatRequestUserMessage(message.Content));
                    }
                }

                // Lastly, add the main prompt last and AFTER the chat history
                // (Rule: System prompt first, chat history second, main prompt last)
                var mainTempate = PromptTemplates.MainPromptTemplate
                   .Replace("{{$prompt}}", mainPrompt.Content);

                // Add main prompt to end of list
                completionOptions.Messages.Add(new ChatRequestUserMessage(mainTempate));

                completionOptions.DeploymentName = _deploymentOrModelName35; // _deploymentOrModelName;
                completionOptions.Temperature = ezCompletionOptions.Temperature;
                completionOptions.MaxTokens = ezCompletionOptions.MaxTokens;
                completionOptions.FrequencyPenalty = ezCompletionOptions.FrequencyPenalty;
                completionOptions.PresencePenalty = ezCompletionOptions.PresencePenalty;
                completionOptions.NucleusSamplingFactor = ezCompletionOptions.Nucleus;

                // Call the LLM
                var completionResponse = await client.GetChatCompletionsAsync(completionOptions);

                // Add subsquent call to LLM to generate followup questions
                var followupTemplate = PromptTemplates.SugestionsPromptTemplate
                    .Replace("{{$answer}}", completionResponse.Value.Choices.FirstOrDefault()?.Message?.Content);


                // Subsequent call to LLM to generate followup questions
                // Note here, we invoke the CompletionAsync, not the ChatCompletionAsync,as we're not passing in chat history
                var suggestions = await client.GetChatCompletionsAsync(new ChatCompletionsOptions // ChatCompletionsOptions
                {
                    DeploymentName = _deploymentOrModelName35,
                    Temperature = ezCompletionOptions.Temperature,
                    MaxTokens = ezCompletionOptions.MaxTokens,
                    FrequencyPenalty = ezCompletionOptions.FrequencyPenalty,
                    PresencePenalty = ezCompletionOptions.PresencePenalty,
                    NucleusSamplingFactor = ezCompletionOptions.Nucleus,
                    Messages = { new ChatRequestUserMessage(followupTemplate) }
                });

                // Reconstruct Optimized ChatHistory to return to caller
                
                // Add System Prompt to beginning of list
                chatMessages.Insert(0, new ChatMessage(Role.System, PromptTemplates.SystemPromptTemplate, 0));
                
                // Right after System Prompt, insert the main prompt and answer
                chatMessages.Insert(1, new ChatMessage(Role.User, 
                                                       mainPrompt.Content, 
                                                       completionResponse.Value.Usage.PromptTokens));

                // Right after the main prompt, insert the completion response 
                chatMessages.Insert(2, new ChatMessage(Role.Assistant, 
                                                       completionResponse.Value.Choices.FirstOrDefault()?.Message?.Content, 
                                                       completionResponse.Value.Usage.CompletionTokens));

                //var followupPromptTokens = suggestions.Value.Usage.PromptTokens;
                //var followupTotalTokens = suggestions.Value.Usage.TotalTokens;

                //var completionPromptTokens = completionResponse.Value.Usage.PromptTokens;
                //var completionTotalTokens = completionResponse.Value.Usage.TotalTokens;

                //var totalPromptTokens = completionPromptTokens + followupPromptTokens;
                //var totalTotalTokens = completionTotalTokens + followupTotalTokens;

                //return (completionResponse.Value,
                //        suggestions.Value,
                //        completionResponse.Value.Usage.PromptTokens,
                //        completionResponse.Value.Usage.CompletionTokens,
                //        suggestions.Value.Usage.TotalTokens);


                return new Completion
                {
                    Response = completionResponse.Value.Choices.FirstOrDefault()?.Message?.Content,
                    Suggestions = suggestions.Value.Choices.FirstOrDefault()?.Message?.Content,
                    PromptTokens = completionResponse.Value.Usage.PromptTokens,
                    ResponseTokens = completionResponse.Value.Usage.CompletionTokens,
                    SuggestionTokens = suggestions.Value.Usage.TotalTokens,
                    ChatHistory = chatMessages
                };

            }
            catch (Exception ex)
            {
                var errorMessage = $"Exception throw in ChatBot.ChatCompletionAsync: {ex.Message}";
                _logger.LogError(errorMessage);
                throw;
            }
        }
    }
}

