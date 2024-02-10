using Azure.AI.OpenAI;
using Shared.Models;

namespace Contracts
{
    public interface IChatCompletion
    {
        Task<Completion> ChatCompletionAsync(EZCompletionOptions ezCompletionOptions);
    }
}