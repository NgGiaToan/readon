using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadOn.DbContexts;
using ReadOn.Dtos;
using ReadOn.Helpers;
using ReadOn.Services;
using ReadOn.Shared;

namespace ReadOn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("dashboard-totaluser")]
        public async Task<IActionResult> GetTotal()
        {
            int totalUser = await _accountService.TotalUsers();
            return Ok(totalUser);
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("dashboard-admins")]
        public async Task<IActionResult> Admins()
        {
            List<AdminDto> admins = await _accountService.AdminAsync();
            return Ok(admins);
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("user-manager")]
        public async Task<IActionResult> Users()
        {
            List<UserDto> user = await _accountService.UserAsync();
            return Ok(user);
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("view-user")]
        public async Task<IActionResult> ViewUser(Guid id)
        {
            var user = await _accountService.ViewUser(id);
            return Ok(user);
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser(CreateUpdateUserDto value, Guid id)
        {
            ApiResponse<ApplicationAccount> user = await _accountService.CreateUser(value, id);
            return StatusCode(user.StatusCode, new { success = user.Success, message = user.Message, data = user.Data });
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(CreateUpdateUserDto value, Guid id)
        {
            ApiResponse<ApplicationAccount> user = await _accountService.UpdateUser(value, id);
            return StatusCode(user.StatusCode, new {succes = user.Success, message = user.Message, data = user.Data});  
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            ApiResponse<bool> user = await _accountService.DeleteUser(id);
            return StatusCode(user.StatusCode, new { succes = user.Success, message = user.Message, data = user.Data });
        }
    }
}
