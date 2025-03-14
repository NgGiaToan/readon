using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ReadOn.Models;
using ReadOn.Services;
using ReadOn.Shared;

namespace ReadOn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;
        private readonly IEmailService _emailService;
        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;

        }

        [HttpPost("signup")]
        public async Task<ApiResponse<bool>> SignUp(Signup model)
        {
            var result = await _authService.SignUpAsync(model);

            return new ApiResponse<bool>(result.Success, result.Message, result.Data, result.StatusCode);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin(Signin model)
        {
            try
            {
                var (accessToken, refreshToken) = await _authService.SigninAsync(model);
                return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
            }
            catch (Exception ex) {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(Guid id)
        {
            var result = await _authService.LogoutAsync(id);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string token)
        {
            try
            {
                var (accessToken, refreshToken) = await _authService.RefreshTokenAsync(token);
                return Ok(new {AccessToken = accessToken, RefreshToken = refreshToken});
            }
            catch (Exception ex) { 
                return Unauthorized(new {message = ex.Message});
            }
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(Guid id,string oldPW, string newPW)
        {
            var result = await _authService.ChangePassword(id, oldPW, newPW);
            return Ok(result);
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(string username)
        {
            var result = await _authService.SendOTP(username);
            return Ok(result);
        }

        [HttpPost("check-otp")]
        public async Task<IActionResult> CheckOtp(string username, string otp)
        {
            var resut = await _authService.CheckOTP(username, otp);
            return Ok(resut);
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(string username, string resetToken, string newPassword)
        {
            var result = await _authService.ResetPassword(username, resetToken, newPassword);
            return Ok(result);
        }
    }
}
