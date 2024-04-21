using Shared.Models;

namespace Shared.Contracts
{
    public interface IDataExtractionCompletion
    {
        Task<Completion> ChatCompletionAsync(CompletionOverrides ezCompletionOptions);
    }
}