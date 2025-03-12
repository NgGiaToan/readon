using Microsoft.AspNetCore.Http;
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
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("SignUp")]
        public async Task<ApiResponse<bool>> SignUp(Signup model)
        {
            var result = await _authService.SignUpAsync(model);

            return new ApiResponse<bool>(result.Success, result.Message, result.Data, result.StatusCode);
        }

        [HttpPost("SignIn")] 
        public async Task<IActionResult> Signin(Signin model)
        {
            var result = await _authService.SignInAsync(model);
            if (string.IsNullOrEmpty(result))
            {
                return Unauthorized();
            }
            return Ok(result);
        }
    }
}
