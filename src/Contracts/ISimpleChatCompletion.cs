using Azure.AI.OpenAI;
using Shared.Models;

namespace Contracts
{
    public interface ISimpleChatCompletion
    {
        Task<Completion> ChatCompletionAsync(EZCompletionOptions ezCompletionOptions);
    }
}