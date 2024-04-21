using UnstructuredRAG.Service.Options;

namespace UnstructuredRAG.Service.Contracts
{
    public interface IOpenAiService
    {
        //Task<(string response, int promptTokens, int responseTokens)> GetChatCompletionAsync(string sessionId, string userPrompt);

        Task<(string response, int promptTokens, int responseTokens)>
            PostChatCompletionAsync(string sessionId, CompletionOptions completionOptions, string userPrompt, string correlationToken);
        Task<string> SummarizeAsync(string sessionId, string userPrompt, string correlationToken);
    }
}