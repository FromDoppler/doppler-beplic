using DopplerBeplic.Models;
using DopplerBeplic.Models.Config;
using DopplerBeplic.Models.DTO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DopplerBeplic.Services.Classes
{
    public class BeplicService
    {
        private readonly BeplicSdk _sdk;
        private readonly DefaulValuesOptions _options;

        public BeplicService(IOptions<DefaulValuesOptions> options, BeplicSdk sdk)
        {
            _options = options.Value;
            _sdk = sdk;
        }
        public UserCreationResponse CreateUser(UserCreationDTO accountData)
        {
            accountData.Room ??= new UserCreationRoom
            {
                RoomName = _options.Room.Name,
                Group = new RoomGroup
                {
                    GroupName = _options.Room.Group
                }
            };

            accountData.Plan ??= new UserCreationPlan
            {
                IdPlan = _options.Plan.Id,
                PlanName = _options.Plan.Name,
                MessageLimit = _options.Plan.MessageLimit
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
