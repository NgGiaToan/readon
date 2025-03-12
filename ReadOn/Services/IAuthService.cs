using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ReadOn.Models;
using ReadOn.Shared;

namespace ReadOn.Services
{
    public interface IAuthService
    {
        Task<string> SignInAsync(Signin model);
        Task<ApiResponse<bool>> SignUpAsync(Signup model);
    }
}
