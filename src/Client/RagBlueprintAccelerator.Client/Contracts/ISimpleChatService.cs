using Azure.AI.OpenAI;
using Shared.Models;

namespace RagBlueprintAccelerator.Client.Contracts
{
    public interface ISimpleChatService
    {
        Task<Completion> PostChatCompletion(CompletionOverrides completionOptions);
    }
}