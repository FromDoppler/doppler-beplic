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
            accountData.Customer.Partner ??= _options.Customer.Partner;
            accountData.Customer.BusinessName ??= _options.Customer.BusinessName;
            accountData.Customer.LegalName ??= _options.Customer.LegalName;
            accountData.Customer.Address ??= _options.Customer.Address;
            accountData.Customer.Cuit ??= _options.Customer.Cuit;
            accountData.Customer.Plan ??= new UserCreationPlan
            {
                IdPlan = _options.Plan.Id,
                PlanName = _options.Plan.Name,
                MessageLimit = _options.Plan.MessageLimit
            };

            var result = new UserCreationResponse();

            try
            {
                var response = await _sdk.ExecuteResource("/services/beplicconfigurationintegra/v1/integra/customers", accountData, RestSharp.Method.Post);

                if (response.IsSuccessStatusCode)
                {
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? string.Empty, new
                    {
                        success = false,
                        message = string.Empty,
                        data = new
                        {
                            idCustomer = 0,
                            accessToken = string.Empty
                        }
                    });

                    result.Success = deserealizedResponse?.success ?? false;
                    result.Error = result.Success ? null : deserealizedResponse?.message;
                    result.CustomerId = deserealizedResponse?.data.idCustomer;
                    result.UserToken = deserealizedResponse?.data.accessToken;
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

        public async Task<CustomerUpdateResponse> UpdateCustomer(CustomerUpdateDTO customerData)
        {
            customerData.Customer.BusinessName ??= _options.Customer.BusinessName;
            customerData.Customer.LegalName ??= _options.Customer.LegalName;
            customerData.Customer.Address ??= _options.Customer.Address;
            customerData.Customer.Cuit ??= _options.Customer.Cuit;
            customerData.Customer.Status = ParseCustomerStatus(customerData);

            var result = new CustomerUpdateResponse();

            try
            {
                var response = await _sdk.ExecuteResource("/services/beplicconfigurationintegra/v1/integra/customers", customerData, RestSharp.Method.Put);

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
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? string.Empty,
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

        private string ParseCustomerStatus(CustomerUpdateDTO customerData)
        {
            if (string.IsNullOrEmpty(customerData.Customer.Status))
            {
                customerData.Customer.Status = customerData.Customer.IsCancelated ?? false
                    ? _options.CustomerStatus.Low
                    : customerData.Customer.IsBlocked ?? false ? _options.CustomerStatus.Inactive : _options.CustomerStatus.Active;
            }

            return customerData.Customer.Status;
        }

        public async Task<UserAdminUpdateResponse> UpdateUserAdmin(UserAdminUpdateDTO userAdminData)
        {
            var result = new UserAdminUpdateResponse();

            try
            {
                var response = await _sdk.ExecuteResource("/services/beplicconfigurationintegra/v1/integra/user", userAdminData, RestSharp.Method.Put);

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
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? string.Empty,
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
