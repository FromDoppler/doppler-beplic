using System.Net;
using System.Security.Authentication;
using Newtonsoft.Json;
using RestSharp;

namespace DopplerBeplic.Services.Classes
{
    public class BeplicSdk
    {
        private const string API_URL = "https://qa.beplic.io/";
        public string? AccessToken { get; private set; }
        public DateTime? ExpirationDate { get; private set; }

        private readonly RestClient _client;
        public BeplicSdk()
        {
            _client = GetClient();
            Authenticate();
        }

        public RestResponse PostResource(string resource, object body)
        {
            EnsureAuthentication();

            var request = new RestRequest(resource, Method.Post);
            request.AddJsonBody(body);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + AccessToken);

            return _client.Execute(request);
        }

        private static RestClient GetClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;

            return new RestClient(API_URL);
        }

        private void Authenticate()
        {
            var request = new RestRequest("auth/login", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                username = "integraciones@fromdoppler.com",
                password = "******",
                rememberMe = false
            }, ContentType.Json);

            var response = _client.Execute(request);

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            dynamic content = response.IsSuccessStatusCode
                ? JsonConvert.DeserializeAnonymousType(response.Content ?? string.Empty,
                    new
                    {
                        access_token = string.Empty,
                        expires_in = 0
                    })
                : throw new AuthenticationException(message: "Failed to authenticate to beplic API.");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            AccessToken = content.access_token.ToString();
            ExpirationDate = DateTime.UtcNow.AddSeconds(content.expires_in);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private void EnsureAuthentication()
        {
            if (AccessToken is null || (ExpirationDate is not null && ExpirationDate < DateTime.UtcNow))
            {
                Authenticate();
            }
        }
    }
}
