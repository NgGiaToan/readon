using ReadOn.DbContexts;
using ReadOn.Dtos;
using ReadOn.Shared;

namespace ReadOn.Services
{
    public interface IAccountService
    { 
        Task<int> TotalUsers();
        Task<List<AdminDto>> AdminAsync();
        Task<List<UserDto>> UserAsync();
        Task<ViewUserDto> ViewUser(Guid id);
        Task<ApiResponse<ApplicationAccount>> CreateUser(CreateUpdateUserDto value, Guid id);
        Task<ApiResponse<ApplicationAccount>> UpdateUser(CreateUpdateUserDto value, Guid id);
        Task<ApiResponse<bool>> DeleteUser(Guid id);
    }
}
