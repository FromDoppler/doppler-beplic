using DopplerBeplic.Models.DTO;
using DopplerBeplic.Models.Responses;

namespace DopplerBeplic.Services.Interfaces
{
    public interface IBeplicService
    {
        Task<UserCreationResponse> CreateUser(UserCreationDTO accountData);

        Task<CompanyUpdateResponse> UpdateCompany(CompanyUpdateDTO customerData);

        Task<UserAdminUpdateResponse> UpdateUserAdmin(UserAdminUpdateDTO userAdminData);
    }
}
