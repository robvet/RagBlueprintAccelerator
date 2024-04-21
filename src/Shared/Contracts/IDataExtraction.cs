using Microsoft.AspNetCore.Http;
using Shared.Models;

namespace Shared.Contracts
{
    public interface IDataExtraction
    {
        Task<UploadDocumentsResponse> ExtractDataFromPDF(IEnumerable<IFormFile> files);
    }
}