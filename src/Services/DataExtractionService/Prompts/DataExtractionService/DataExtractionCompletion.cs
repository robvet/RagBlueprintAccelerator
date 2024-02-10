using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Contracts;
using Shared.Models;
using Shared.Enums;
using DataExtractionService.Prompts.DataExtractionService.Prompts;

namespace DataExtractionService.Prompts.DataExtractionService
{
    public class DataExtractionCompletion : ISimpleChatCompletion, IDataExtractionCompletion
    {
        private readonly ILogger<DataExtractionCompletion> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _endpoint;
        private readonly string _key;
        private readonly string _deploymentOrModelName4;
        private readonly string _deploymentOrModelName35;

        public DataExtractionCompletion(IConfiguration configuration, ITokenManager tokenManager)
        {
            _configuration = configuration;

            // Get the Azure OpenAI Service configuration values
            _endpoint = _configuration["AzureOpenAIServiceOptions:Endpoint"] ?? throw new ArgumentException("AzureOpenAIServiceOptions:Endpoint is Missing");
            _deploymentOrModelName4 = _configuration["AzureOpenAIServiceOptions:DeploymentOrModelName"] ?? throw new ArgumentException("AzureOpenAIServiceOptions:DeploymentOrModelName is Missing");
            _deploymentOrModelName35 = _configuration["AzureOpenAIServiceOptions:DeploymentOrModelName35"] ?? throw new ArgumentException("AzureOpenAIServiceOptions:DeploymentOrModelName35 is Missing");
            _key = _configuration["AzureOpenAIServiceOptions:Key"] ?? throw new ArgumentException("AzureOpenAIServiceOptions:Key is Missing"); ;
        }

        //public async Task<(ChatCompletions response, ChatCompletions followup, int promptTokens, int completionTokens, int suggestionTokens)> ChatCompletionAsync(EZCompletionOptions ezCompletionOptions)
        public async Task<Completion> ChatCompletionAsync(EZCompletionOptions ezCompletionOptions)
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
                var mainPrompt = chatMessages.LastOrDefault();

                // Remove the last message from the chat history as it will be the main prompt.
                chatMessages.RemoveAt(chatMessages.Count - 1);

                // Build the completion options object 
                var completionOptions = new ChatCompletionsOptions();

                // Add the system prompt first
                completionOptions.Messages.Add(systemPrompt);

                //// Add the chat history
                //foreach (var message in ezCompletionOptions.ChatMessages.Take(ezCompletionOptions.ChatMessages.Count))
                //{
                //    if (message.Role == Role.Assistant)
                //    {
                //        completionOptions.Messages.Add(new ChatRequestAssistantMessage(message.Content));
                //    }
                //    else
                //    {
                //        completionOptions.Messages.Add(new ChatRequestUserMessage(message.Content));
                //    }
                //}

                // Lastly, add the main prompt last and AFTER the chat history
                // (Rule: System prompt first, chat history second, main prompt last)
                var mainTempate = PromptTemplates.MainPromptTemplate
                   .Replace("{{$prompt}}", mainPrompt.Content);

                // Add main prompt to end of list
                completionOptions.Messages.Add(new ChatRequestUserMessage(mainTempate));

                completionOptions.DeploymentName = _deploymentOrModelName4; //_deploymentOrModelName35; 
                completionOptions.Temperature = ezCompletionOptions.Temperature;
                completionOptions.MaxTokens = ezCompletionOptions.MaxTokens;
                completionOptions.FrequencyPenalty = ezCompletionOptions.FrequencyPenalty;
                completionOptions.PresencePenalty = ezCompletionOptions.PresencePenalty;
                completionOptions.NucleusSamplingFactor = ezCompletionOptions.Nucleus;

                // Call the LLM
                var completionResponse = await client.GetChatCompletionsAsync(completionOptions);


                // Reconstruct Optimized ChatHistory to return to caller

                // Right after System Prompt, insert the main prompt and answer
                chatMessages.Insert(0, new ChatMessage(Role.User,
                                                       mainPrompt.Content,
                                                       completionResponse.Value.Usage.PromptTokens));

                // Right after the main prompt, insert the completion response 
                chatMessages.Insert(1, new ChatMessage(Role.Assistant,
                                                       completionResponse.Value.Choices.FirstOrDefault()?.Message?.Content,
                                                       completionResponse.Value.Usage.CompletionTokens));

                return new Completion
                {
                    Response = completionResponse.Value.Choices.FirstOrDefault()?.Message?.Content,
                    PromptTokens = completionResponse.Value.Usage.PromptTokens,
                    ResponseTokens = completionResponse.Value.Usage.CompletionTokens,
                    ChatHistory = chatMessages
                };

            }
            catch (Exception ex)
            {
                var errorMessage = $"Exception throw in DataExtractionService.DataExtractionCompletion: {ex.Message}";
                _logger.LogError(errorMessage);
                throw;
            }
        }
    }
}


//**********************************************************************************************************************
//* Code Junkyard
//**********************************************************************************************************************

//var junk = "";
//var chatHistory = new List<ChatRequestMessage>();

//foreach (var message in ezCompletionOptions.ChatMessage.Take(ezCompletionOptions.ChatMessage.Count - 1))
//{
//    if (message.Role == Role.User)
//    {
//        chatHistory.Add(new ChatRequestUserMessage(message.Content));
//    }
//    else
//    {
//        chatHistory.Add(new ChatRequestAssistantMessage(message.Content));
//    }
//}

//ezCompletionOptions.ChatMessage.ForEach(x => new ChatRequestAssistantMessage(x))

//// want to parse the chat history into a list of ChatRequestAssistantMessage or ChatRequestUserMessage based on the role property of the ChatMessage object
////var chatHistory = new List<ChatRequestAssistantMessage>();
//ezCompletionOptions.ChatMessage.ForEach(x => chatHistory.Add(new ChatRequestAssistantMessage(x)));

//var chatHistory = new ChatRequestAssistantMessage(ezCompletionOptions.ChatHistory);


//List<ChatRequestMessage> chatRequestMessages = ezCompletionOptions.ChatMessage
//    .Select(x => x.Role == "user"
//        ? (ChatRequestMessage)new ChatRequestUserMessage(x.Content)
//        : new ChatRequestAssistantMessage(x.Content))
//    .ToList();

//// Build the completion options
//var completionOptions = new ChatCompletionsOptions();
//completionOptions.Messages.Add(systemPrompt);
////completionOptions.Messages.Add(prompt);

////foreach (var chatRequestMessage in chatRequestMessages)
////{
////    completionOptions.Messages.Add(chatRequestMessage);
////}

//completionOptions.DeploymentName = _deploymentOrModelName;

//completionOptions.Temperature = ezCompletionOptions.Temperature;
//completionOptions.MaxTokens = ezCompletionOptions.MaxTokens;
//completionOptions.FrequencyPenalty = ezCompletionOptions.FrequencyPenalty;
//completionOptions.PresencePenalty = ezCompletionOptions.PresencePenalty;
//completionOptions.NucleusSamplingFactor = ezCompletionOptions.Nucleus;

//chatRequest.Items = chatHistory;
//chatRequest.CurrentTurn = chatHistory.Count - 1; // Index of the most recent turn

//var completionResponse = await client.GetChatCompletionsAsync(chatHistory, prompt, systemPrompt);



//chatHistory

//});

//var completionResponse = await client.GetChatCompletionsAsync(new ChatCompletionsOptions
//    {
//        DeploymentName = _deploymentOrModelName,
//        Temperature = ezCompletionOptions.Temperature,
//        MaxTokens = ezCompletionOptions.MaxTokens,
//        FrequencyPenalty = ezCompletionOptions.FrequencyPenalty,
//        PresencePenalty = ezCompletionOptions.PresencePenalty,
//        NucleusSamplingFactor = ezCompletionOptions.Nucleus,
//        Messages = new List<ChatRequestMessage> { systemPrompt, prompt }.Concat(chatRequestMessages).ToList()
//    });

// experiment

////// ... (previous code for storing chat history)

////var chatHistory2 = new List<ChatMessage>()
////{
////    new ChatMessage { Role = "user", Content = "Hi there!" },
////    new ChatMessage { Role = "assistant", Content = "Hello! How can I help you?" },
////    // ... (more chat history)
////};

////var chatCompletionRequest = new ChatCompletionRequest();
////chatCompletionRequest.ChatRequestUserMessage = new ChatRequestUserMessage
////{
////    Role = "user",
////    Text = "Can you suggest a good restaurant?"
////};
////chatCompletionRequest.Messages = chatHistory2;

////var completion = await client.ChatCompletions.PostAsync(chatCompletionRequest);