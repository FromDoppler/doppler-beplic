using System.Net;
using System.Security.Authentication;
using DopplerBeplic.Models.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace DopplerBeplic.Services.Classes
{
    public class BeplicSdk
    {
        private readonly ApiConnectionOptions _options;
        public string? AccessToken { get; private set; }
        public DateTime? ExpirationDate { get; private set; }

        private readonly RestClient _client;
        public BeplicSdk(IOptions<ApiConnectionOptions> options)
        {
            _options = options.Value;
            _client = GetClient();
        }

        public async Task<RestResponse> ExecuteResource(string resource, object body, Method metod)
        {
            await EnsureAuthentication();

            var request = new RestRequest(resource, metod);
            request.AddJsonBody(body);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + AccessToken);

            return await _client.ExecuteAsync(request);
        }
        private RestClient GetClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;

            return new RestClient(_options.BaseUrl);
        }

        private async Task Authenticate()
        {
            var request = new RestRequest("auth/login", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                username = _options.User,
                password = _options.Password,
                rememberMe = false
            }, ContentType.Json);

            var response = await _client.ExecuteAsync(request);

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

        private async Task EnsureAuthentication()
        {
            if (AccessToken is null || (ExpirationDate is not null && ExpirationDate < DateTime.UtcNow))
            {
                await Authenticate();
            }
        }
    }
}
