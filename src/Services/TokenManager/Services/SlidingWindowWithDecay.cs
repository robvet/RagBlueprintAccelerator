using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Contracts;
using Shared.Enums;
using Shared.Models;
using TokenManager.Prompts;

namespace TokenManager.Services
{
    public class SlidingWindowWithDecay : ITokenManager
    {
        private readonly ILogger<SlidingWindowWithDecay> _logger;
        private readonly IConfiguration _configuration;

        private readonly string _endpoint;
        private readonly string _key;
        private readonly string _deploymentOrModelName4;
        private readonly string _deploymentOrModelName35;

        // Base Completion Options as suggested by ChatGPT-4
        private readonly float _temperature = 0.60F;
        private readonly float _nucleus = 0.9F;
        private readonly int _maxTokens;
        private readonly float _frequencyPenalty = 0.6F;
        private readonly float _PresencePenalty = 0.6F;

        public SlidingWindowWithDecay(IConfiguration configuration, ILogger<SlidingWindowWithDecay> logger)
        {
            _logger = logger;
            _configuration = configuration;
            
            // Get the Azure OpenAI Service configuration values
            _endpoint = _configuration["AzureOpenAIServiceOptions_Endpoint"] ?? throw new ArgumentException("AzureOpenAIServiceOptions_Endpoint is Missing");
            _deploymentOrModelName4 = _configuration["AzureOpenAIServiceOptions_DeploymentOrModelName"] ?? throw new ArgumentException("AzureOpenAIServiceOptions_DeploymentOrModelName is Missing");
            _deploymentOrModelName35 = _configuration["AzureOpenAIServiceOptions_DeploymentOrModelName35"] ?? throw new ArgumentException("AzureOpenAIServiceOptions_DeploymentOrModelName35 is Missing");
            _key = _configuration["AzureOpenAIServiceOptions_Key"] ?? throw new ArgumentException("AzureOpenAIServiceOptions_Key is Missing"); ;
        }

        /// <summary>
        /// Implement Sliding Window with Decay pattern to optimize the chat history
        /// </summary>
        /// <param name="chatMessages"></param>
        /// <returns>Optimzied Chat History</returns>
        //https://devblogs.microsoft.com/surface-duo/android-openai-chatgpt-16/
        public async Task<List<ChatMessage>> OptimizeChatHistoryAsync(List<ChatMessage> chatMessages)
        {
            try
            {
                // Represents the average number of tokens that are expected
                // for the prompt and chathistory for a typical chatturn.
                // Once maxTokens exceeds the limit, the LLM will not return further responses.
                var maxInputTokens = CompletionOptions.AverageInputTokens;
                
                // Represents the maximum size of chat history that will be used
                int historyMax = (int)(maxInputTokens * .6F);
                // Represents the maximum size of summaried chat history that will be used
                int summaryMax = (int) (maxInputTokens * .4F);

                string assistantOverflowContext = string.Empty;
                string userOverflowContext = string.Empty; 

                int tokensUsed = 0;

                List<ChatMessage> fullChatHistory = chatMessages;
                List<ChatMessage> trimmedChatHistory = [];

                // Remove main message which is last message in collection as it's not part of the history
                //chatHistory.RemoveAt(chatHistory.Count - 1);

                // Start at the end of fullchat history and work backwards
                // in reverse order as to remove the oldest messages first
                for (int i = fullChatHistory.Count - 1; i >= 0; i--)
                {
                    // We're storing number of tokens for each conversation as
                    // provided by OpenAI in the ChatHistory collection
                    tokensUsed += fullChatHistory[i].Tokens;

                    // Iterate through each chat message
                    if (tokensUsed <= historyMax)
                    {
                        // Keep chat message intact if its beneath token amount allowed for chat history
                        trimmedChatHistory.Add(fullChatHistory[i]);
                    }
                    else
                    {
                        //Chat message exceeds token amount allowed for chat history
                        // Concatenate extra messages to pass to LLM for summarization
                        if (fullChatHistory[i].Role == Role.Assistant)
                        {
                            assistantOverflowContext += fullChatHistory[i].Content + Environment.NewLine + Environment.NewLine;
                        }
                        else if (fullChatHistory[i].Role == Role.User)
                        {
                            userOverflowContext += fullChatHistory[i].Content + Environment.NewLine + Environment.NewLine;
                        }   
                    }
                }

                // Add the trimmed chat history to the collection
                if (!string.IsNullOrWhiteSpace(assistantOverflowContext))
                {
                    // Summarize Assistant chat messages that were dropped due to max conversation tokens
                    trimmedChatHistory.Add(new ChatMessage
                    (
                        Role.Assistant,
                        await ContextCondensorAsync(assistantOverflowContext, summaryMax),
                        0
                    ));
                }

                if (!string.IsNullOrWhiteSpace(userOverflowContext))
                {
                    // Summarize User chat messages that were dropped due to max conversation tokens
                    trimmedChatHistory.Add(new ChatMessage
                    (
                        Role.User,
                        await ContextCondensorAsync(userOverflowContext, summaryMax),
                        0
                    ));
                }

                // Invert the chat messages to put back into chronological order and output as string.        
                //var reversedChatHistory = trimmedChatHistory;
                //reversedChatHistory.Reverse();
                //ezCompletionOptions.ChatMessage = reversedChatHistory;
                //return ezCompletionOptions;

                // Invert the chat messages back into chronological order
                trimmedChatHistory.Reverse();
                return trimmedChatHistory;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Exception throw in ChatBot.ChatCompletionAsync: {ex.Message}";
                _logger.LogError(errorMessage);
                throw;
            }
        }

        /// <summary>
        /// Summarize the chat history that was dropped due to max conversation tokens
        /// Passed to  LLM for summarization and return as string.
        /// </summary>
        /// <param name="chatHistory"></param>
        /// <returns>string of summarized context</returns>
        private async Task<string> ContextCondensorAsync(string chatHistory, double maxSummarizedHistoryTokens)
        {
            try
            {
                // Summarize the chat history that was dropped due to max conversation tokens
                // and return as a string to be passed to the LLM for summarization.

                OpenAIClient client = new OpenAIClient(
                    new Uri(_endpoint),
                    new AzureKeyCredential(_key));

                // Build system and main prompts for prompt templates
                // Convert both to ChatRequestMessage objects
                var systemPrompt = new ChatRequestSystemMessage(PromptTemplates.SystemPromptTemplate);

                // Main prompt is the current text request from the user
                var mainTempate = PromptTemplates.MainPromptTemplate
                    .Replace("{{$prompt}}", chatHistory);
                var mainPrompt = new ChatRequestUserMessage(mainTempate);

                var summaryCompletion = await client.GetChatCompletionsAsync(new ChatCompletionsOptions
                {
                    DeploymentName = _deploymentOrModelName35,
                    Temperature = _temperature,
                    MaxTokens = (int?)maxSummarizedHistoryTokens,
                    FrequencyPenalty = _frequencyPenalty,
                    PresencePenalty = _PresencePenalty,
                    NucleusSamplingFactor = _nucleus,
                    Messages = {
                        new ChatRequestSystemMessage(PromptTemplates.SystemPromptTemplate),
                        new ChatRequestUserMessage(mainTempate)
                    }
                });

                return summaryCompletion.Value.Choices.FirstOrDefault()?.Message?.Content;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Exception throw in SlidingWindow.ContextCondensor: {ex.Message}";
                _logger.LogError(errorMessage);
                throw;
            }
        }
    }
}




