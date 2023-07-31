using DopplerBeplic.Models.Config;
using DopplerBeplic.Models.DTO;
using DopplerBeplic.Models.Responses;
using DopplerBeplic.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DopplerBeplic.Services.Classes
{
    public class BeplicService : IBeplicService
    {
        private readonly BeplicSdk _sdk;
        private readonly DefaulValuesOptions _options;

        public BeplicService(IOptions<DefaulValuesOptions> options, BeplicSdk sdk)
        {
            _options = options.Value;
            _sdk = sdk;
        }
        public async Task<UserCreationResponse> CreateUser(UserCreationDTO accountData)
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

            var result = new UserCreationResponse();

            try
            {
                var response = await _sdk.ExecuteResource("v1/integra/customer", accountData, RestSharp.Method.Post);

                if (response.IsSuccessStatusCode)
                {
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? "", new
                    {
                        success = false,
                        message = string.Empty,
                        data = new
                        {
                            idCustomer = 0
                        }
                    });

                    result.Success = deserealizedResponse?.success ?? false;
                    result.Error = result.Success ? string.Empty : deserealizedResponse?.message;
                    result.CustomerId = deserealizedResponse?.data.idCustomer;
                }
                else
                {
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? "",
                        new
                        {
                            errors = new[]
                            {
                                new {
                                    status = string.Empty,
                                    title = string.Empty,
                                    detail = string.Empty,
                                    source = new
                                    {
                                        pointer = string.Empty
                                    }
                                }
                            }.ToList()
                        });

                    //TODO: Verify with beplic if the array of errors it's realy needed.
                    var error = deserealizedResponse?.errors.FirstOrDefault();

                    result.Success = false;
                    result.ErrorStatus = error?.status;
                    result.Error = error?.detail;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }

            return result;
        }

        public async Task<CompanyUpdateResponse> UpdateCompany(CompanyUpdateDTO customerData)
        {
            var result = new CompanyUpdateResponse();

            try
            {
                var response = await _sdk.ExecuteResource("v1/integra/customer", customerData, RestSharp.Method.Put);

                if (response.IsSuccessStatusCode)
                {
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? "", new
                    {
                        success = false,
                        message = string.Empty,
                        data = new
                        {
                            idCustomer = 0
                        }
                    });

                    result.Success = deserealizedResponse?.success ?? false;
                    result.Error = result.Success ? string.Empty : deserealizedResponse?.message;
                    result.CustomerId = deserealizedResponse?.data.idCustomer;
                }
                else
                {
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? "",
                        new
                        {
                            errors = new[]
                            {
                                new {
                                    status = string.Empty,
                                    title = string.Empty,
                                    detail = string.Empty,
                                    source = new
                                    {
                                        pointer = string.Empty
                                    }
                                }
                            }.ToList()
                        });

                    //TODO: Verify with beplic if the array of errors it's realy needed.
                    var error = deserealizedResponse?.errors.FirstOrDefault();

                    result.Success = false;
                    result.ErrorStatus = error?.status;
                    result.Error = error?.detail;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }

            return result;
        }

        public async Task<UserAdminUpdateResponse> UpdateUserAdmin(UserAdminUpdateDTO userAdminData)
        {
            var result = new UserAdminUpdateResponse();

            try
            {
                var response = await _sdk.ExecuteResource("v1/integra/user", userAdminData, RestSharp.Method.Put);

                if (response.IsSuccessStatusCode)
                {
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? "", new
                    {
                        success = false,
                        message = string.Empty,
                        data = new
                        {
                            idCustomer = 0,
                            idUser = 0
                        }
                    });

                    result.Success = deserealizedResponse?.success ?? false;
                    result.Error = result.Success ? string.Empty : deserealizedResponse?.message;
                    result.CustomerId = deserealizedResponse?.data.idCustomer;
                    result.UserId = deserealizedResponse?.data.idUser;
                }
                else
                {
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? "",
                        new
                        {
                            errors = new[]
                            {
                                new {
                                    status = string.Empty,
                                    title = string.Empty,
                                    detail = string.Empty,
                                    source = new
                                    {
                                        pointer = string.Empty
                                    }
                                }
                            }.ToList()
                        });

                    //TODO: Verify with beplic if the array of errors it's realy needed.
                    var error = deserealizedResponse?.errors.FirstOrDefault();

                    result.Success = false;
                    result.ErrorStatus = error?.status;
                    result.Error = error?.detail;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }

            return result;
        }
    }
}
