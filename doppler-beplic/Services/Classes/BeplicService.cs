using System.Net;
using DopplerBeplic.Models;
using DopplerBeplic.Models.DTO;
using Newtonsoft.Json;
using RestSharp;

namespace DopplerBeplic.Services.Classes
{
    public class BeplicService
    {
        private readonly BeplicSdk _sdk = new BeplicSdk();
        public UserCreationResponse CreateUser(UserCreationDTO accountData)
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

            var response = _sdk.PostResource("v1/integra/customer", accountData);

            var result = new UserCreationResponse();
            if (response.IsSuccessStatusCode)
            {
                result.Success = true;
                dynamic? deserealizedResponse = JsonConvert.DeserializeObject(response.Content ?? "");
                result.CustomerId = deserealizedResponse?.data?.idCustomer;
            }
            else
            {
                result.Success = false;
                dynamic? deserealizedResponse = JsonConvert.DeserializeObject(response.Content ?? "");
                result.ErrorStatus = deserealizedResponse?.errors?.status;
                result.Error = deserealizedResponse?.errors?.detail;
            }

            return result;
        }
    }
}
