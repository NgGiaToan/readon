using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadOn.DbContexts;
using ReadOn.Dtos;
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

        [HttpGet("DBGetTotal")]
        public async Task<IActionResult> GetTotal()
        {
            int totalBranch = await _branchService.TotalBranch();
            return Ok(totalBranch);
        }

        [HttpGet("DBBranches")]
        public async Task<IActionResult> Branches()
        {
            List<ItemDto> branches = await _branchService.Branches();
            return Ok(branches);
        }

        [HttpGet("Branches")]
        public async Task<IActionResult> BranchDto()
        {
            List<BranchDto> branches = await _branchService.BranchAsync();
            return Ok(branches);
        }

        [HttpGet("ViewBranch")]
        public async Task<IActionResult> ViewBranchAsync(Guid id)
        {
            ViewBranchDto branch = await _branchService.ViewBranch(id);
            return Ok(branch);
        }

        [HttpPost("CreateBranch")]
        public async Task<IActionResult> CreateBranchAsync(CreateUpdateBranchDto value, Guid id)
        {
            var branch = await _branchService.CreateBranch(value, id);
            return StatusCode(branch.StatusCode, new { success = branch.Success, message = branch.Message, data = branch.Data });
        }

        [HttpPut("UpdateBranch")]
        public async Task<IActionResult> UpdateBranchAsync(CreateUpdateBranchDto value, Guid id)
        {
            var branch = await _branchService.UpdateBranch(value, id);
            return StatusCode(branch.StatusCode, new { success = branch.Success, message = branch.Message, data = branch.Data });
        }

        [HttpDelete("DeleteBranch")]
        public async Task<IActionResult> DeleteRBranchAsync(Guid id)
        {
            var branch = await _branchService.DeleteBranch(id);
            return StatusCode(branch.StatusCode, new { success = branch.Success, message = branch.Message, data = branch.Data });
        }
    }
}
