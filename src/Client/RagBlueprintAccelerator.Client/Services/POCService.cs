using System.Net.Http.Json;
using Shared.Models;

namespace RagBlueprintAccelerator.Client.Services
{
    public class POCService(HttpClient httpClient) : IPOCService
    {
        public async Task<string> CallPOCServiceGet()
        {
            try
            {
                //var response = httpClient.GetStringAsync("api/POC");
                var response = await httpClient.GetFromJsonAsync<string>("api/POC");

                return response;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }

        public async Task<string> CallPOCServicePost()
        {
            try
            {
                var customer = new Customer
                {
                    Name = "Test",
                    Email = "Test@Test.com"
                };

                var customerId = 1;

                var response = await httpClient.PostAsJsonAsync($"api/POC?Id={customerId}", customer);

                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }
    }
}
