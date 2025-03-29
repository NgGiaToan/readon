using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadOn.DbContexts;
using ReadOn.Dtos;
using ReadOn.Helpers;
using ReadOn.Services;

namespace ReadOn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;
        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("dashboard-totalbranch")]
        public async Task<IActionResult> GetTotal()
        {
            int totalBranch = await _branchService.TotalBranch();
            return Ok(totalBranch);
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("dashboard-branches")]
        public async Task<IActionResult> Branches()
        {
            List<ItemDto> branches = await _branchService.Branches();
            return Ok(branches);
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("branches")]
        public async Task<IActionResult> BranchDto(string? n )
        {
            List<BranchDto> branches = await _branchService.BranchAsync(n);
            return Ok(branches);
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("view-branch")]
        public async Task<IActionResult> ViewBranchAsync(Guid id)
        {
            ViewBranchDto branch = await _branchService.ViewBranch(id);
            return Ok(branch);
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpPost("create-branch")]
        public async Task<IActionResult> CreateBranchAsync(CreateUpdateBranchDto value, Guid id)
        {
            var branch = await _branchService.CreateBranch(value, id);
            return StatusCode(branch.StatusCode, new { success = branch.Success, message = branch.Message, data = branch.Data });
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpPut("update-branch")]
        public async Task<IActionResult> UpdateBranchAsync(CreateUpdateBranchDto value, Guid id)
        {
            var branch = await _branchService.UpdateBranch(value, id);
            return StatusCode(branch.StatusCode, new { success = branch.Success, message = branch.Message, data = branch.Data });
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpDelete("delete-branch")]
        public async Task<IActionResult> DeleteRBranchAsync(Guid id)
        {
            var branch = await _branchService.DeleteBranch(id);
            return StatusCode(branch.StatusCode, new { success = branch.Success, message = branch.Message, data = branch.Data });
        }
    }
}
