using Azure;
using Azure.AI.OpenAI;
//using Shared.Models;
using UnstructuredRAG.Service.Contracts;
using UnstructuredRAG.Service.Options;

namespace UnstructuredRAG.Service.Services;


/// <summary>
/// Service to access Azure OpenAI SDK.
/// </summary>
public class OpenAiService : IOpenAiService
{
    private readonly string _modelName = string.Empty;
    private readonly OpenAIClient _client;
    private readonly ILogger<OpenAiService> _logger;

    private readonly string _searchEndpoint;
    private readonly string _searchKey;
    private readonly string _searchIndex;

    public OpenAiService(ILogger<OpenAiService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="endpoint">Endpoint URI.</param>
    /// <param name="key">Account key.</param>
    /// <param name="modelName">Name of the deployed Azure OpenAI model.</param>
    /// <param name="searchEndpoint">Endpoint for Azure Cognitive Search instance.</param>
    /// <param name="searchKey">Key for Azure Cognitive Search instance.</param>
    /// <param name="searchIndex">Index for Azure Cognitive Search instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when endpoint, key, or modelName is either null or empty.</exception>
    /// <remarks>
    /// Creates OpenAI Object
    /// </remarks>
    public OpenAiService(string endpoint, string key, string modelName, string searchEndpoint, string searchKey, string searchIndex)

    {
        ArgumentException.ThrowIfNullOrEmpty(modelName);
        ArgumentException.ThrowIfNullOrEmpty(endpoint);
        ArgumentException.ThrowIfNullOrEmpty(key);

        ArgumentException.ThrowIfNullOrEmpty(searchEndpoint);
        ArgumentException.ThrowIfNullOrEmpty(searchKey);
        ArgumentException.ThrowIfNullOrEmpty(searchIndex);

        _modelName = modelName;

        // Instantiates microsoft OpenAIClient class
        _client = new(new Uri(endpoint), new AzureKeyCredential(key));

        _searchEndpoint = searchEndpoint;
        _searchKey = searchKey;
        _searchIndex = searchIndex;
    }

    // System prompt to send with user prompts to instruct the model for chat session
    //private readonly string _systemPrompt = @"
    //    You are an AI assistant that helps people find information.
    //    Provide concise answers that are polite and professional." + Environment.NewLine;

    private readonly string _systemPrompt = string.Empty;
    //@"You are an AI assistant that helps application architects and developers better understand the Azure platform. You're job is to provide information about Azure products and services based on their questions. Your response should not exceed 100 words and should include links to reference material in the form of URLs. For any question that don't concern Azure products and services, respond with \""Sorry, but I cannot help you with that question. Please ask me a question about Azure Products and Services\" + Environment.NewLine;

    // otherwise, return the citations. The citations should be in the form of a list of URLs. The URLs should be clickable links. The citations should be in the form of a list of URLs. The URLs should be clickable links. The citations should be in the form of a list of URLs. The URLs should be clickable links. The citations should be in the form of a list of URLs. The URLs should be clickable links. The citations should be in the form of a list of URLs. The URLs should be clickable links. The citations should be in the form of a list of URLs. The URLs should be clickable links." + Environment.NewLine;
    //    You are an AI assistant that helps people find information.
    //    Provide concise answers that are polite and professional." + Environment.NewLine;

    // System prompt to send with user prompts to instruct the model for summarization
    private readonly string _summarizePrompt = @"
            Summarize this prompt in one or two words to use as a label in a button on a web page" + Environment.NewLine;

    private int _tokenLimit = 3000;
    private float _temperature = 0.3f;
    private float _nucleus = 0.5f;

    /// <summary>
    /// Send prompt to the deployed OpenAI LLM model and return completion.
    /// </summary>
    /// <param name="sessionId">Chat session identifier for the current conversation.</param>
    /// <param name="userPrompt">Prompt message to send to the deployment.</param>
    /// <param name="completionOptions">Collection of values to adjust completion senstivitiy.</param>
    /// <param name="correlationToken">Unique tracking value for the request</param>
    /// <returns>Response from the OpenAI model along with tokens for the prompt and response.</returns>
    public async Task<(string response, int promptTokens, int responseTokens)>
            PostChatCompletionAsync(string sessionId, CompletionOptions completionOptions, string userPrompt, string correlationToken)
    {
        try
        {
            // Assign system prompt from request or default value, if null
            ////ChatMessage systemMessage = new(ChatRole.System, completionOptions.SystemPrompt ?? _systemPrompt);
            //ChatMessage systemMessage = new(ChatRole.System, _systemPrompt);
            ChatRequestSystemMessage systemMessage = new(_systemPrompt);
            ChatRequestUserMessage userMessage = new(userPrompt);

            var cognitiveSearchIndexFieldMappingOptions = new AzureCognitiveSearchIndexFieldMappingOptions()
            {
                UrlFieldName = "url",
                FilepathFieldName = "filepath"
            };

            ChatCompletionsOptions options = new()
            {
                Messages =
                {
                    systemMessage,
                    userMessage
                },

                User = sessionId,
                MaxTokens = completionOptions.TokenLimit > 0 ? completionOptions.TokenLimit : _tokenLimit,

                // Temperature controls 'creativity' of generated completion.
                // Higher values output more random and creative responses.
                // Lower values will make results more focused, conserative, and deterministic.
                // Valid range is float between 0.0 and 2.0; default value is 1.0
                Temperature = completionOptions.Temperature > 2.0f ? completionOptions.Temperature : _temperature,

                // Nucleus sampling causes model to consider the results of the tokens with NucleusSamplingFactor probability.
                // Valid range is float between 0.0 and 1.0; default value is 1.0
                NucleusSamplingFactor = completionOptions.Nucleus > 1.0f ? completionOptions.Nucleus : _nucleus,

                // Discourages model from repeating the same words or phrases too frequently within the generated text.
                // A higher frequency_penalty value will result in the model being more conservative in its use of repeated tokens.
                // Decreases likelihood of the output repeating itself verbatim. 
                // Range between -2.0 and 2.0
                // Default value is 0.0
                FrequencyPenalty = 0,

                // Encourags to include diverse range of tokens in the generated text.
                // A higher presence_penalty value will result in the model being more likely to generate tokens
                // that have not yet been included in the generated text.
                // Range between -2.0 and 2.0
                // Default value is 0.0
                PresencePenalty = 0,

                AzureExtensionsOptions = new AzureChatExtensionsOptions()
                {
                    Extensions =
                    {
                        new AzureCognitiveSearchChatExtensionConfiguration()
                        {
                            SearchEndpoint = new Uri(_searchEndpoint),
                            IndexName = _searchIndex,
                            //SearchKey = new AzureKeyCredential(_searchKey),
                            ShouldRestrictResultScope = false,
                            DocumentCount =1,
                            FieldMappingOptions = cognitiveSearchIndexFieldMappingOptions
                        }
                    }
                }
            };

            // Retrieve conversation, including latest prompt.
            // If you put this after the vector search it doesn't take advantage of previous information given so harder to chain prompts together.
            // However if you put this before the vector search it can get stuck on previous answers and not pull additional information. Worth experimenting
            //https://github.com/Azure/Vector-Search-AI-Assistant/blob/cognitive-search-vector/VectorSearchAiAssistant.Service/Services/ChatService.cs#L92

            //Response<ChatCompletions> completionsResponse = await _client.GetChatCompletionsAsync(_modelName, options);
            Response<ChatCompletions> completionsResponse = await _client.GetChatCompletionsAsync(options);

            var message = completionsResponse.Value.Choices[0].Message;

            //foreach (var contextMessage in message.AzureExtensionsContext.Messages)
            //{
            //    // Note: citations and other extension payloads from the "tool" role are often encoded JSON documents

            //    // Extract the field mapping option for URLs
            //    string urlFieldName = cognitiveSearchIndexFieldMappingOptions.UrlFieldName;

            //    // Extract the value of urlFieldName
            //    string urlFieldNameValue = cognitiveSearchIndexFieldMappingOptions.UrlFieldName;

            //    // Print the extracted value
            //    Console.WriteLine("URL field name value: " + urlFieldNameValue);

            //}

            ChatCompletions completions = completionsResponse.Value;

            return (
                response: completions.Choices[0].Message.Content,
                promptTokens: 0, //completions.Usage.PromptTokens,
                responseTokens: 0// completions.Usage.CompletionTokens
            );
        }
        catch (Exception ex)
        {
            var errorMessage = "Exception throw PostChatCompletionAsyn() with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Sends the existing conversation to the OpenAI model and returns a two word summary.
    /// </summary>
    /// <param name="sessionId">Chat session identifier for the current conversation.</param>
    /// <param name="userPrompt">Prompt conversation to send to the deployment.</param>
    /// <param name="correlationToken">Unique tracking value for the request</param>
    /// <returns>Summarization response from the OpenAI model deployment.</returns>
    public async Task<string> SummarizeAsync(string sessionId, string userPrompt, string correlationToken)
    {
        try
        {
            //ChatMessage systemMessage = new(ChatRole.System, _summarizePrompt);
            //ChatMessage userMessage = new(ChatRole.User, userPrompt);

            ChatRequestSystemMessage systemMessage = new(_summarizePrompt);
            ChatRequestUserMessage userMessage = new(userPrompt);

            ChatCompletionsOptions options = new()
            {
                Messages = {
                    systemMessage,
                    userMessage
                },
                User = sessionId,
                MaxTokens = 200,
                Temperature = 0.0f,
                NucleusSamplingFactor = 1.0f,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };

            //Response<ChatCompletions> completionsResponse = await _client.GetChatCompletionsAsync(_modelName, options);
            Response<ChatCompletions> completionsResponse = await _client.GetChatCompletionsAsync(options);

            ChatCompletions completions = completionsResponse.Value;


            string summary = completions.Choices[0].Message.Content;

            return summary;
        }
        catch (Exception)
        {
            var errorMessage = "Exception throw SummarizeAsync() with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }
}
