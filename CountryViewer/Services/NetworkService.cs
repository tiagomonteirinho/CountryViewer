using System.Net;
using CountryViewer.Models;

namespace CountryViewer.Services;

public class NetworkService
{
    public Response CheckConnection()
    {
        var client = new WebClient();

        try
        {
            using (client.OpenRead("http://clients3.google.com/generate_204"))
            {
                return new Response
                {
                    IsSuccessful = true
                };
            }
        }
        catch
        {
            return new Response
            {
                IsSuccessful = false
            };
        }
    }
}
