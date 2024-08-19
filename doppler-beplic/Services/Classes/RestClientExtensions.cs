using RestSharp;

namespace DopplerBeplic.Services.Classes
{
    public static class RestClientExtensions
    {
        public static async Task<RestResponse> ExecuteResource(this IRestClient client, string accessToken, string resource, object body, Method method)
        {
            var request = new RestRequest(resource, method);
            request.AddJsonBody(body);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + accessToken);

            return await client.ExecuteAsync(request);
        }

        public static async Task<RestResponse> ExecuteResource(this IRestClient client, string accessToken, string resource, Parameter[] parameters, Method method)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + accessToken);
            request.Parameters.AddParameters(parameters);

            return await client.ExecuteAsync(request);
        }

        public static async Task<RestResponse> ExecuteResource(this IRestClient client, string accessToken, string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + accessToken);

            return await client.ExecuteAsync(request);
        }
    }
}
