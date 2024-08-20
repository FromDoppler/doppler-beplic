using System.Globalization;
using System.Net;
using DopplerBeplic.Models.Config;
using DopplerBeplic.Models.DTO;
using DopplerBeplic.Models.Responses;
using DopplerBeplic.Models.Responses.BeplicResponses;
using DopplerBeplic.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace DopplerBeplic.Services.Classes
{
    public partial class BeplicService : IBeplicService
    {
        private readonly BeplicSdk _sdk;
        private readonly DefaulValuesOptions _options;
        private readonly ILogger<BeplicService> _logger;

        public BeplicService(IOptions<DefaulValuesOptions> options, BeplicSdk sdk, ILogger<BeplicService> logger)
        {
            _options = options.Value;
            _sdk = sdk;
            _logger = logger;
        }

        [LoggerMessage(0, LogLevel.Information, "Unsuccesfull response from Beplic API for UserId:{UserId}. Response: {Response} Status: {Status}")]
        partial void LogInfoBadRequest(string userId, string response, string status);

        [LoggerMessage(1, LogLevel.Error, "Unexpected exception thrown for UserId:{UserId}.")]
        partial void LogErrorException(string userId, Exception e);

        [LoggerMessage(1, LogLevel.Error, "Error from method: {method}. Message: {message}.")]
        partial void LogErrorMethod(string method, string message);

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
                var response = await _sdk.ExecuteApiResource("/services/beplicpartners/v1/integra/customers", accountData, RestSharp.Method.Post);

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
                    LogInfoBadRequest(accountData.Customer.IdExternal, response.Content ?? "", response.StatusCode.ToString());
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? "", new ErrorResponse());

                    result.Success = false;
                    result.ErrorStatus = deserealizedResponse?.Status;
                    result.Error = deserealizedResponse?.Message;
                }
            }
            catch (Exception ex)
            {
                LogErrorException(accountData.Customer.IdExternal, ex);
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
                var response = await _sdk.ExecuteApiResource("/services/beplicpartners/v1/integra/customers", customerData, RestSharp.Method.Put);

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
                    LogInfoBadRequest(customerData.Customer.IdExternal, response.Content ?? "", response.StatusCode.ToString());
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? string.Empty, new ErrorResponse());

                    result.Success = false;
                    result.ErrorStatus = deserealizedResponse?.Status;
                    result.Error = deserealizedResponse?.Message;
                }
            }
            catch (Exception ex)
            {
                LogErrorException(customerData.Customer.IdExternal, ex);
                result.Success = false;
                result.Error = ex.Message;
            }

            return result;
        }

        public async Task<PlanBalanceResponse> GetPlanBalance(string idExternal)
        {
            var result = new PlanBalanceResponse();

            try
            {
                var parameters = new Parameter[]
                {
                    Parameter.CreateParameter("idExternal",idExternal,ParameterType.QueryString)
                };

                var response = await _sdk.ExecuteApiResource("/services/beplicpartners/v1/integra/plan/balance", parameters, Method.Get);

                if (response.IsSuccessStatusCode)
                {
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? "", new
                    {
                        success = false,
                        message = string.Empty,
                        data = new
                        {
                            conversationsQtyBalance = 0,
                            whatsAppCreditBalance = (decimal?)0.0
                        }
                    });

                    result.Success = deserealizedResponse?.success ?? false;
                    result.Error = result.Success ? string.Empty : deserealizedResponse?.message;
                    result.ConversationsQtyBalance = deserealizedResponse?.data.conversationsQtyBalance;
                    result.WhatsAppCreditBalance = deserealizedResponse?.data.whatsAppCreditBalance;
                }
                else
                {
                    LogInfoBadRequest(idExternal.ToString(), response.Content ?? "", response.StatusCode.ToString());
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? string.Empty, new ErrorResponse());

                    result.Success = false;
                    result.ErrorStatus = deserealizedResponse?.Status;
                    result.Error = deserealizedResponse?.Message;
                }
            }
            catch (Exception ex)
            {
                LogErrorException(idExternal, ex);
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
                var response = await _sdk.ExecuteApiResource("/services/beplicpartners/v1/integra/user", userAdminData, RestSharp.Method.Put);

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
                    LogInfoBadRequest(userAdminData.IdExternal, response.Content ?? "", response.StatusCode.ToString());
                    var deserealizedResponse = JsonConvert.DeserializeAnonymousType(response.Content ?? string.Empty, new ErrorResponse());

                    result.Success = false;
                    result.ErrorStatus = deserealizedResponse?.Status;
                    result.Error = deserealizedResponse?.Message;
                }
            }
            catch (Exception ex)
            {
                LogErrorException(userAdminData.IdExternal, ex);
                result.Success = false;
                result.Error = ex.Message;
            }

            return result;
        }

        public async Task<IEnumerable<PlanResponse>> GetPlans()
        {
            try
            {
                var response = await _sdk.ExecuteServiceResource("/services/beplicoreuser/api/v1/plans", Method.Get);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<BeplicServiceResponse<List<PlanResponse>>>(response.Content ?? string.Empty);

                    if (result is not null && result.HttpStatusCode == (int)HttpStatusCode.OK)
                    {
                        return result.Data ?? [];
                    }

                    LogInfoBadRequest("Get plans", result?.Message ?? "", result?.HttpStatusCode.ToString(CultureInfo.InvariantCulture) ?? "");

                    throw new BadHttpRequestException(result?.Message ?? "");
                }
                else
                {
                    LogInfoBadRequest("Get plans", response.Content ?? "", response.StatusCode.ToString());

                    throw new BadHttpRequestException(response.Content ?? "");
                }
            }
            catch (Exception ex)
            {
                LogErrorException("Get plans", ex);
                throw;
            }
        }

        public async Task<PlanAssignResponse> PlanAssign(PlanAssignmentDTO planAssignData)
        {
            var result = new PlanAssignResponse();

            try
            {
                var planAssignBeplicDto = new
                {
                    partner = _options.Customer.Partner,
                    idExternal = planAssignData.IdExternal,
                    idPlan = planAssignData.IdPlan,
                };

                var response = await _sdk.ExecuteServiceResource("/services/beplicoreuser/api/v1/plans/assign-plan", planAssignBeplicDto, Method.Post);

                if (response.IsSuccessStatusCode)
                {
                    var deserealizedResponse = JsonConvert.DeserializeObject<BeplicServiceResponse<BeplicPlanAssignSuccessResponse>>(response.Content ?? string.Empty);

                    result.Success = true;
                    result.StartDate = deserealizedResponse?.Data?.StartDate;
                    result.EndDate = deserealizedResponse?.Data?.EndDate;
                    result.ActiveDate = deserealizedResponse?.Data?.ActiveDate;
                    result.Active = deserealizedResponse?.Data?.Active;
                    result.TrialPeriod = deserealizedResponse?.Data?.TrialPeriod;
                }
                else
                {
                    var deserealizedResponse = JsonConvert.DeserializeObject<BeplicServiceResponse<dynamic>>(response.Content ?? string.Empty);

                    result.Success = false;
                    result.Error = deserealizedResponse?.Message ?? "Unknown error";
                    result.ErrorStatus = deserealizedResponse?.HttpStatusCode.ToString(CultureInfo.InvariantCulture) ?? "";

                    LogInfoBadRequest(planAssignData.IdExternal, response.Content ?? "", result.ErrorStatus);
                }
            }
            catch (Exception ex)
            {
                LogErrorException("Plan assign", ex);
                throw;
            }

            return result;
        }

        public async Task<TemplateMessageResponse> SendTemplateMessage(int roomId, int templateId, TemplateMessageDTO messageDTO)
        {
            var bodyParams = new
            {
                channel = "1",
                roomId,
                templateId,
                phoneNumber = messageDTO.PhoneNumber,
                phoneNumberBusiness = messageDTO.PhoneNumberBusiness,
                link = messageDTO.Link,
                headerType = "TEXT",
                parameterHeader = messageDTO?.HeaderParameters?.Length > 0 ? messageDTO.HeaderParameters[0] : null,
                parametersBody = messageDTO?.BodyParameters
            };

            var response = await _sdk.ExecuteServiceResource("/services/partner/conversation/templates/message-out", bodyParams, Method.Post);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<BeplicServiceResponse<BeplicTemplateMessageResponse>>(response.Content ?? string.Empty);

                return new TemplateMessageResponse()
                {
                    MessageId = result?.Data?.MessageId
                };
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<BeplicServiceResponse<dynamic>>(response.Content ?? string.Empty);

                var message = errorResponse?.Message ?? string.Empty;
                var statusCode = errorResponse?.HttpStatusCode ?? (int)HttpStatusCode.InternalServerError;

                LogErrorMethod("SendTemplateMessage", response.Content ?? message);

                throw new BadHttpRequestException(message, statusCode);
            }
        }

        public async Task<IEnumerable<RoomResponse>> GetRoomsByCustomer(int idExternal, int channelId)
        {
            var parameters = new Parameter[]
                {
                    Parameter.CreateParameter("idExternal",idExternal,ParameterType.QueryString),
                    Parameter.CreateParameter("channelId", channelId, ParameterType.QueryString)
                };
            var response = await _sdk.ExecuteServiceResource("/services/partner/rooms", parameters, Method.Get);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<BeplicServiceResponse<IEnumerable<BeplicRoomResponse>>>(response.Content ?? string.Empty);

                Func<BeplicRoomResponse, RoomResponse> selector = x => new RoomResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    PhoneNumber = x.Phone
                };

                return result?.Data?.Select(selector) ?? Enumerable.Empty<RoomResponse>();
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<BeplicServiceResponse<dynamic>>(response.Content ?? string.Empty);

                var message = errorResponse?.Message ?? string.Empty;
                var statusCode = errorResponse?.HttpStatusCode ?? (int)HttpStatusCode.InternalServerError;

                LogErrorMethod("GetRoomsByCustomer", response.Content ?? message);

                throw new BadHttpRequestException(message, statusCode);
            }
        }

        public async Task<IEnumerable<TemplateResponse>> GetTemplatesByRoom(int roomId)
        {
            var bodyParams = new
            {
                roomId
            };

            var response = await _sdk.ExecuteServiceResource("/services/partner/templates/room", bodyParams, Method.Post);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<BeplicServiceResponse<IEnumerable<BeplicTemplateResponse>>>(response.Content ?? string.Empty);

                Func<BeplicTemplateResponse, TemplateResponse> selector = x => new TemplateResponse()
                {
                    BodyAmount = x.BodyAmount ?? 0,
                    BodyText = x.BodyText,
                    Category = x.Category,
                    FooterText = x.FooterText,
                    HeaderAmount = x.HeaderAmount ?? 0,
                    HeaderText = x.HeaderText,
                    HeaderType = x.HeaderType,
                    Id = x.Id,
                    Name = x.Name,
                    Language = x.Language,
                    PublicPreviewUrl = _sdk.GetServiceFullUrl("/external-template", new Parameter[] {
                        Parameter.CreateParameter("bodyText", x.BodyText,ParameterType.QueryString),
                        Parameter.CreateParameter("footerText", x.FooterText, ParameterType.QueryString),
                        Parameter.CreateParameter("headerText", x.HeaderText, ParameterType.QueryString),
                        Parameter.CreateParameter("botonPhone", x.BotonPhone, ParameterType.QueryString),
                        Parameter.CreateParameter("botonUrl", x.BotonUrl, ParameterType.QueryString),
                        Parameter.CreateParameter("labelPhone", x.LabelPhone, ParameterType.QueryString),
                        Parameter.CreateParameter("labelUrl", x.LabelUrl, ParameterType.QueryString),
                        Parameter.CreateParameter("parameterHeader", x.ParameterHeader, ParameterType.QueryString),
                        Parameter.CreateParameter("parameter1Body", x.Parameter1Body, ParameterType.QueryString),
                        Parameter.CreateParameter("parameter2Body", x.Parameter2Body, ParameterType.QueryString),
                        Parameter.CreateParameter("parameter3Body", x.Parameter3Body, ParameterType.QueryString),
                        Parameter.CreateParameter("quickReply1", x.QuickReply1, ParameterType.QueryString),
                        Parameter.CreateParameter("quickReply2", x.QuickReply2, ParameterType.QueryString),
                        Parameter.CreateParameter("quickReply3", x.QuickReply3, ParameterType.QueryString),
                        Parameter.CreateParameter("headerType", x.HeaderType, ParameterType.QueryString),
                        Parameter.CreateParameter("fileId", x.FieldId, ParameterType.QueryString)})
                };

                return result?.Data?.Select(selector) ?? Enumerable.Empty<TemplateResponse>();
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<BeplicServiceResponse<dynamic>>(response.Content ?? string.Empty);

                var message = errorResponse?.Message ?? string.Empty;
                var statusCode = errorResponse?.HttpStatusCode ?? (int)HttpStatusCode.InternalServerError;

                LogErrorMethod("GetTemplatesByRoom", response.Content ?? message);

                throw new BadHttpRequestException(message, statusCode);
            }
        }

        public async Task<PlanCancellationResponse> CancelPlan(string idExternal)
        {
            var result = new PlanCancellationResponse();

            try
            {
                var planCancellationData = new
                {
                    partner = _options.Customer.Partner,
                    idExternal,
                    idPlan = 0,
                };

                var response = await _sdk.ExecuteServiceResource("/services/beplicoreuser/api/v1/plans/assign-plan", planCancellationData, Method.Put);

                if (response.IsSuccessStatusCode)
                {
                    var deserealizedResponse = JsonConvert.DeserializeObject<BeplicServiceResponse<object>>(response.Content ?? string.Empty);

                    result.Success = true;
                }
                else
                {
                    var deserealizedResponse = JsonConvert.DeserializeObject<BeplicServiceResponse<object>>(response.Content ?? string.Empty);

                    result.Success = false;
                    result.Error = deserealizedResponse?.Message ?? "Unknown error";
                    result.ErrorStatus = deserealizedResponse?.HttpStatusCode.ToString(CultureInfo.InvariantCulture) ?? "";

                    LogInfoBadRequest(idExternal, response.Content ?? "", result.ErrorStatus);
                }
            }
            catch (Exception ex)
            {
                LogErrorException("Plan cancellation", ex);
                throw;
            }

            return result;
        }

        public async Task<PlanCreationResponse> CreatePlan(PlanCreationDTO planData)
        {
            var result = new PlanCreationResponse();

            try
            {
                var response = await _sdk.ExecuteServiceResource("/services/beplicoreuser/api/v1/plans", planData, Method.Post);

                var deserealizedResponse = JsonConvert.DeserializeObject<BeplicServiceResponse<PlanResponse>>(response.Content ?? string.Empty);

                if (response.IsSuccessStatusCode)
                {
                    result.Success = true;
                    result.PlanId = deserealizedResponse?.Data?.Id;
                }
                else
                {
                    result.Success = false;
                    result.Error = deserealizedResponse?.Message ?? "Unknown error";
                    result.ErrorStatus = deserealizedResponse?.HttpStatusCode.ToString(CultureInfo.InvariantCulture) ?? "";
                    result.PlanId = deserealizedResponse?.Data?.Id;

                    LogInfoBadRequest(planData.Name, response.Content ?? "", result.ErrorStatus);
                }
            }
            catch (Exception ex)
            {
                LogErrorException(planData.Name, ex);
                result.Success = false;
                result.Error = ex.Message;
                result.PlanId = null;
            }

            return result;
        }
    }
}
