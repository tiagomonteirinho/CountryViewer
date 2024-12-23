using Newtonsoft.Json;
using System.Net.Http;
using CountryViewer.Models;

namespace CountryViewer.Services;

public class ApiService
{
    public async Task<Response> GetCountries(string baseUrl, string controller)
    {
        try
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);

            var response = await client.GetAsync(controller);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return new Response
                {
                    IsSuccessful = false,
                    Message = result,
                };
            }
            else
            {
                var countries = JsonConvert.DeserializeObject<List<Country>>(result);

                return new Response
                {
                    IsSuccessful = true,
                    Result = countries,
                };
            }
        }
        catch (Exception ex)
        {
            return new Response
            {
                IsSuccessful = false,
                Message = ex.Message,
            };
        }

    }
}
