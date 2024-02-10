using Shared.Models;

namespace Contracts
{
    public interface IDataExtractionCompletion
    {
        Task<Completion> ChatCompletionAsync(EZCompletionOptions ezCompletionOptions);
    }
}