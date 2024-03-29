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
    }
}
