using DopplerBeplic.Models.DTO;
using DopplerBeplic.Models.Responses;

namespace DopplerBeplic.Services.Interfaces
{
    public interface IBeplicService
    {
        Task<UserCreationResponse> CreateUser(UserCreationDTO accountData);

        Task<CustomerUpdateResponse> UpdateCustomer(CustomerUpdateDTO customerData);

        Task<UserAdminUpdateResponse> UpdateUserAdmin(UserAdminUpdateDTO userAdminData);

        Task<PlanBalanceResponse> GetPlanBalance(string idExternal);

        Task<IEnumerable<PlanResponse>> GetPlans();

        Task<TemplateMessageResponse> SendTemplateMessage(int roomId, int templateId, TemplateMessageDTO messageDTO);

        Task<IEnumerable<RoomResponse>> GetRoomsByCustomer(int idExternal, int channelId);

        Task<IEnumerable<TemplateResponse>> GetTemplatesByRoom(int roomId);

        Task<PlanCreationResponse> CreatePlan(PlanCreationDTO planData);

        Task<PlanAssignResponse> PlanAssign(PlanAssignmentDTO planAssignData);

        Task<PlanCancellationResponse> CancelPlan(string idExternal);

        Task<UserPlanResponse> GetUserPlan(int idExternal);
    }
}
