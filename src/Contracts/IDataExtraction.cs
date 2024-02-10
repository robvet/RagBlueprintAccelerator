using Microsoft.AspNetCore.Http;
using Shared.Models;

namespace Contracts
{
    public interface IDataExtraction
    {
        Task<UploadDocumentsResponse> ExtractDataFromPDF(IEnumerable<IFormFile> files);
    }
}