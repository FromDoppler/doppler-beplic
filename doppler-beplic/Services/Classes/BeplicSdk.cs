using System.Globalization;
using System.Net;
using System.Security.Authentication;
using DopplerBeplic.Models.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace DopplerBeplic.Services.Classes
{
    public class BeplicSdk
    {
        private readonly ApiConnectionOptions _options;
        public string? AccessToken { get; private set; }
        public DateTime? ExpirationDate { get; private set; }

        private readonly RestClient _apiClient;
        private readonly RestClient _serviceClient;
        public BeplicSdk(IOptions<ApiConnectionOptions> options)
        {
            _options = options.Value;
            _apiClient = GetApiClient();
            _serviceClient = GetServiceClient();
        }

        public async Task<RestResponse> ExecuteApiResource(string resource, object body, Method method)
        {
            await EnsureAuthentication();

            return await _apiClient.ExecuteResource(AccessToken ?? string.Empty, resource, body, method);
        }

        public async Task<RestResponse> ExecuteApiResource(string resource, Parameter[] parameters, Method method)
        {
            await EnsureAuthentication();

            return await _apiClient.ExecuteResource(AccessToken ?? string.Empty, resource, parameters, method);
        }

        public async Task<RestResponse> ExecuteServiceResource(string resource, Method method)
        {
            await EnsureAuthentication();

            return await _serviceClient.ExecuteResource(AccessToken ?? string.Empty, resource, method);
        }

        public async Task<RestResponse> ExecuteServiceResource(string resource, object body, Method method)
        {
            await EnsureAuthentication();

            return await _serviceClient.ExecuteResource(AccessToken ?? string.Empty, resource, body, method);
        }

        public async Task<RestResponse> ExecuteServiceResource(string resource, Parameter[] parameters, Method method)
        {
            await EnsureAuthentication();

            return await _serviceClient.ExecuteResource(AccessToken ?? string.Empty, resource, parameters, method);
        }

        public string GetServiceFullUrl(string resource, Parameter[] parameters)
        {
            var request = new RestRequest(resource, Method.Get);

            request.Parameters.AddParameters(parameters);

            return _serviceClient.BuildUri(request).AbsoluteUri;
        }

        private RestClient GetApiClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;

            return new RestClient(
                _options.BaseApiUrl,
                configureSerialization: s => s.UseNewtonsoftJson());
        }

        private RestClient GetServiceClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;

            return new RestClient(
                _options.BaseServiceUrl,
                configureSerialization: s => s.UseNewtonsoftJson());
        }

        private async Task Authenticate()
        {
            var request = new RestRequest("auth/login", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("partner-key", _options.PartnerKey);

            request.AddJsonBody(new
            {
                username = _options.User,
                password = _options.Password,
                rememberMe = false
            }, ContentType.Json);

            var response = await _apiClient.ExecuteAsync(request);

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            dynamic content = response.IsSuccessStatusCode
                ? JsonConvert.DeserializeAnonymousType(response.Content ?? string.Empty,
                    new
                    {
                        access_token = string.Empty,
                        expires_in = 0
                    })
                : throw new AuthenticationException(message: string.Format(CultureInfo.InvariantCulture, "Failed to authenticate to beplic API. Error: {0}. RequestedURL: {1}", response.Content, response.ResponseUri?.AbsoluteUri ?? ""));
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
