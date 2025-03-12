using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("GetPreview")]
        public async Task<IActionResult> GetPreview(Guid id)
        {
            var result = await _loanPreviewService.GetPreview(id);
            return Ok(result);
        }

        [HttpPost("AddPreview")]
        public async Task<IActionResult> AddPreview(Guid id, Guid bookId)
        {
            var result = await _loanPreviewService.AddPreview(id, bookId);
            return Ok(result);
        }

        [HttpDelete("RemovePreview")]
        public async Task<IActionResult> RemovePreview(Guid id,Guid bookId)
        {
            var result = await _loanPreviewService.RemovePreview(id,bookId);
            return Ok(result);
        }

        [HttpDelete("ClearPreview")]
        public async Task<IActionResult> ClearPreview(Guid id)
        {
            var result = await _loanPreviewService.ClearPreview(id);
            return Ok(result);
        }

    }
}
