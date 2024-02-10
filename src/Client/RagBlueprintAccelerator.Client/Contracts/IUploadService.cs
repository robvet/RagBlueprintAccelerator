using RagBlueprintAccelerator.Client.Models;
using Shared.Models;

namespace RagBlueprintAccelerator.Client.Contracts
{
    public interface IUploadService
    {
        Task<Completion> UploadDocumentsAsync(WebAssemblyTicket ticket, string prompt);
    }
}