using Contracts;
using Microsoft.AspNetCore.Http;
using Shared.Models;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace DataExtractionService.Prompts.DataExtractionService
{
    public class DataExtraction : IDataExtraction
    {
        public async Task<UploadDocumentsResponse> ExtractDataFromPDF(IEnumerable<IFormFile> files)
        {
            // Extract data from the source using the OSS library PdfPig
            try
            {
                List<string> uploadedFiles = [];
                foreach (var file in files)
                {
                    var fileName = file.FileName;

                    await using var stream = file.OpenReadStream();
                    using var pdf = PdfDocument.Open(stream);
                    StringBuilder textBuilder = new StringBuilder();
                    for (int i = 0; i < pdf.NumberOfPages; i++)
                    {
                        Page page = pdf.GetPage(i + 1);
                        string text = page.Text;
                        textBuilder.AppendLine(text);
                    }

                    // PDF file now parsed
                    var textContent = textBuilder.ToString();

                    //// Remove unwanted characters 
                    //// regex pattern to remove extra spaces and new lines
                    //string pattern = @"[ ]{2,}";
                    //Regex regex = new Regex(pattern);
                    //textContent = regex.Replace(textContent, " ");

                    //// Now remove special characters, keeping only letters, numbers, and spaces, perios, and commas
                    //string pattern2 = @"[^a-zA-Z0-9 .,]";
                    //Regex regex2 = new Regex(pattern2);
                    //textContent = regex2.Replace(textContent, "");


                    // Now you have the text of the entire PDF in textBuilder
                    // You can write it to a file or add it to your uploadedFiles list
                    uploadedFiles.Add(textBuilder.ToString());

                }

                return new UploadDocumentsResponse(uploadedFiles.ToArray());
            }
            catch (Exception ex)
            {
                return UploadDocumentsResponse.FromError(ex.ToString());
            }
        }

    }
}


//try
//{
//    List<string> uploadedFiles = [];
//    foreach (var file in files)
//    {
//        var fileName = file.FileName;

//        await using var stream = file.OpenReadStream();

//        using var documents = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
//        for (int i = 0; i < documents.PageCount; i++)
//        {
//            // parse the document



//        }
//    }



//    return new UploadDocumentsResponse([.. uploadedFiles]);
//}


//var documentName = BlobNameFromFilePage(fileName, i);
//var blobClient = container.GetBlobClient(documentName);
//if (await blobClient.ExistsAsync(cancellationToken))
//{
//    continue;
//}

//var tempFileName = Path.GetTempFileName();

//try
//{
//    using var document = new PdfDocument();
//    document.AddPage(documents.Pages[i]);
//    document.Save(tempFileName);

//    await using var tempStream = File.OpenRead(tempFileName);
//    await blobClient.UploadAsync(tempStream, new BlobHttpHeaders
//    {
//        ContentType = "application/pdf"
//    }, cancellationToken: cancellationToken);

//    uploadedFiles.Add(documentName);
//}
//finally
//{
//    File.Delete(tempFileName);
//}


//if (uploadedFiles.Count is 0)
//{
//    return UploadDocumentsResponse.FromError("""
//                    No files were uploaded. Either the files already exist or the files are not PDFs.
//                    """);
//}