using Azure.AI.OpenAI;
using Shared.Models;

namespace RagBlueprintAccelerator.Client.Contracts
{
    public interface IChatService
    {
        Task<Completion> PostChatCompletion(CompletionOverrides completionOptions);
    }
}