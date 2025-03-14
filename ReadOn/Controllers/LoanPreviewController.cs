using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadOn.Helpers;
using ReadOn.Services;

namespace ReadOn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanPreviewController : ControllerBase
    {
        private readonly ILoanPreviewService _loanPreviewService;
        public LoanPreviewController(ILoanPreviewService loanPreviewService)
        {
            _loanPreviewService = loanPreviewService;
        }

        [Authorize]
        [HttpGet("getpreview")]
        public async Task<IActionResult> GetPreview(Guid id)
        {
            var result = await _loanPreviewService.GetPreview(id);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("add-preview")]
        public async Task<IActionResult> AddPreview(Guid id, Guid bookId)
        {
            var result = await _loanPreviewService.AddPreview(id, bookId);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("remove-preview")]
        public async Task<IActionResult> RemovePreview(Guid id,Guid bookId)
        {
            var result = await _loanPreviewService.RemovePreview(id,bookId);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("clear-preview")]
        public async Task<IActionResult> ClearPreview(Guid id)
        {
            var result = await _loanPreviewService.ClearPreview(id);
            return Ok(result);
        }

    }
}
