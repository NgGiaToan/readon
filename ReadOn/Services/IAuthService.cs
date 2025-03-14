using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ReadOn.DbContexts;
using ReadOn.Models;
using ReadOn.Shared;
using System.Threading.Tasks;

namespace ReadOn.Services
{
    public interface IAuthService
    {
        Task<(string accessToken, string refreshToken)> SigninAsync(Signin model);
        Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken);
        Task<string> GenerateAccessToken(ApplicationAccount user);
        string GenerateRefreshToken();
        Task<ApiResponse<bool>> SignUpAsync(Signup model);
        Task<bool> LogoutAsync(Guid userId);
        Task<ApiResponse<bool>> ChangePassword(Guid userId, string oldPassword, string newPassword);
        Task<ApiResponse<bool>> SendOTP(string username);
        Task<ApiResponse<string>> CheckOTP(string username, string otp);
        Task<ApiResponse<bool>> ResetPassword(string username, string resetToken, string newpassword);
    }
}
