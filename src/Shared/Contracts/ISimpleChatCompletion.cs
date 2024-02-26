using Azure.AI.OpenAI;
using Shared.Models;

namespace Shared.Contracts
{
    public interface ISimpleChatCompletion
    {
        Task<Completion> ChatCompletionAsync(CompletionOverrides ezCompletionOptions);
    }
}