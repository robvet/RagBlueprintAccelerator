using System.Net.Http.Json;
using Azure;
using Azure.AI.OpenAI;
using RagBlueprintAccelerator.Client.Contracts;
using Shared.Models;

namespace RagBlueprintAccelerator.Client.Services
{
    public class SimpleChatService(HttpClient httpClient) : ISimpleChatService
    {
        //public async Task<string> CallPOCServiceGet()
        //{
        //    try
        //    {
        //        //var response = httpClient.GetStringAsync("api/POC");
        //        var response = await httpClient.GetFromJsonAsync<string>("api/POC");

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        var error = ex.Message;
        //        throw;
        //    }
        //}

        public async Task<Completion> PostChatCompletion(EZCompletionOptions completionOptions)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync($"api/simplechat", completionOptions);

                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();

                    // if status code 400 or greater, thrown exception so that
                    // exception handler takes care of it
                    if ((int)response.StatusCode == 400)
                    {
                        var error = $"Status Code of 400 returned in UI RestClient: {message}";
                        //_logger.LogError(error);
                        throw new HttpRequestException(error);
                    }

                    if ((int)response.StatusCode == 404)
                    {
                        //logger.LogError($"Status Code of 404 returned in UI RestClient: {message}");
                        var error = $"{response.StatusCode}:{message}";
                        throw new HttpRequestException(error);
                    }

                    if ((int)response.StatusCode >= 500)
                    {
                        var errorMessage = $"Status Code of 500 returned in UI RestClient Post(): {message}";
                        //_logger.LogError(errorMessage);
                        throw new HttpRequestException(errorMessage);
                    }
                }

                var result = await response.Content.ReadFromJsonAsync<Completion>();
                return result;


                //return await response.Content.ReadFromJsonAsync<Game>(); ;
                //return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }
    }
}
