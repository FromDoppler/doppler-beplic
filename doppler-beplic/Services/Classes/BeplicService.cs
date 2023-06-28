// unset:error

using DopplerBeplic.Models;
using DopplerBeplic.Models.DTO;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace DopplerBeplic.Services.Classes
{
    public class BeplicService
    {
        public UserCreationResponse CreateUser(UserCreationDTO accountData, string authToken)
        {
            accountData.Room ??= new UserCreationRoom
                {
                    RoomName = "Sala A",
                    Group = new RoomGroup
                    {
                        GroupName = "Grupo A"
                    }
                };

            accountData.Plan ??= new UserCreationPlan
            {
                IdPlan = 5,
                PlanName = "free",
                MessageLimit = 1000
            };

            var response = PostResource<UserCreationDTO>("/customer", accountData, authToken);

            var result = new UserCreationResponse();
            if (response.IsSuccessStatusCode)
            {
                result.Success = true;
                dynamic? deserealizedResponse = JsonConvert.DeserializeObject(response.Content ?? "");
                result.CustomerId = deserealizedResponse?.data?.idCustomer;
            }
            else
            {
                result.Success=false;
                dynamic? deserealizedResponse = JsonConvert.DeserializeObject(response.Content ?? "");
                result.ErrorStatus = deserealizedResponse?.errors?.status;
                result.Error = deserealizedResponse?.errors?.detail;
            }

            return result;
        }

        private static RestClient GetClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;

            var client = new RestClient("https://api.beplic.io/v1/integra/");
            return client;
        }

        private static RestResponse PostResource<T> (string resource, object body, string authToken)
        {
            var client = GetClient();
            var request = new RestRequest(resource, Method.Post);
            request.AddJsonBody(body);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", authToken);

            return client.Execute(request);
        }
    }
}
