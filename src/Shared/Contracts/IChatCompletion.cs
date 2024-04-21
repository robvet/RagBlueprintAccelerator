using Azure.AI.OpenAI;
using Shared.Models;

namespace Shared.Contracts
{
    public interface IChatCompletion
    {
        Task<Completion> ChatCompletionAsync(CompletionOverrides ezCompletionOptions);
    }
}