using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadOn.DbContexts;
using ReadOn.Dtos;
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
        
        [HttpGet("DBGetTotalUser")]
        public async Task<IActionResult> GetTotal()
        {
            int totalUser = await _accountService.TotalUsers();
            return Ok(totalUser);
        }

        [HttpGet("DBAdmins")]
        public async Task<IActionResult> Admins()
        {
            List<AdminDto> admins = await _accountService.AdminAsync();
            return Ok(admins);
        }

        [HttpGet("Users")]
        public async Task<IActionResult> Users()
        {
            List<UserDto> user = await _accountService.UserAsync();
            return Ok(user);
        }

        [HttpGet("ViewUser")]
        public async Task<IActionResult> ViewUser(Guid id)
        {
            var user = await _accountService.ViewUser(id);
            return Ok(user);
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(CreateUpdateUserDto value, Guid id)
        {
            ApiResponse<ApplicationAccount> user = await _accountService.CreateUser(value, id);
            return StatusCode(user.StatusCode, new { success = user.Success, message = user.Message, data = user.Data });
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(CreateUpdateUserDto value, Guid id)
        {
            ApiResponse<ApplicationAccount> user = await _accountService.UpdateUser(value, id);
            return StatusCode(user.StatusCode, new {succes = user.Success, message = user.Message, data = user.Data});  
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            ApiResponse<bool> user = await _accountService.DeleteUser(id);
            return StatusCode(user.StatusCode, new { succes = user.Success, message = user.Message, data = user.Data });
        }
    }
}
