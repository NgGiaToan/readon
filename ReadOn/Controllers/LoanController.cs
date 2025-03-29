using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadOn.DbContexts;
using ReadOn.Dtos;
using ReadOn.Helpers;
using ReadOn.Services;
using System.Threading.Tasks;

namespace ReadOn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("dashboard-statistics")]
        public async Task<IActionResult> GetLoanStatistics()
        {
            var statistics = await _loanService.GetLoanStatisticsAsync();
            return Ok(statistics);
        }

        [Authorize]
        [HttpGet("user-statistics")]
        public async Task<IActionResult> GetUserLoanStatistics(Guid id)
        {
            var user_statistics = await _loanService.GetUserLoanStatisticsAsync(id);
            return Ok(user_statistics);
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("dashboard-overdueborrowers")]
        public async Task<IActionResult> GetDBOverdueBorrowers()
        {
            List<ItemDto> dboverdueborrowers = await _loanService.DBOverdueBorrowersAsync();
            return Ok(dboverdueborrowers);
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("overdueborrowers")]
        public async Task<IActionResult> GetOverdueBorrowers(string? n)
        {
            List<BorrowedDto> overborrowers = await _loanService.OverdueBorrowersAsync(n);
            return Ok(overborrowers);
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("borrowed")]
        public async Task<IActionResult> GetBorrowed(string? n)
        {
            List<BorrowedDto> overborrowers = await _loanService.BorrowedAsync(n);
            return Ok(overborrowers);
        }

        [Authorize]
        [HttpGet("loandetail")]
        public async Task<IActionResult> GetOverdueBorrowersDetail(Guid id)
        {
            List<BookInLoanDetailDto> bookDetailDtos = await _loanService.LoanDetailAsync(id);
            return Ok(bookDetailDtos);
        }

        [Authorize]
        [HttpGet("user-borrowedbook")]
        public async Task<IActionResult> GetUserBorrowed(Guid id, string? n) 
        {
            var borrowed = await _loanService.UserBorrowedAsync(id, n);
            return Ok(borrowed);
        }

        [Authorize]
        [HttpGet("user-returnedbook")]
        public async Task<IActionResult> GetUserReturned(Guid id, string? n)
        {
            var borrowed = await _loanService.UserReturnedAsync(id, n);
            return Ok(borrowed);
        }

        [Authorize]
        [HttpPut("user-returned")]
        public async Task<IActionResult> UserReturnedAsync(Guid id)
        {
            var result = await _loanService.UserReturnedDetailAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("user-borrewed")]
        public async Task<IActionResult> CreateBorrowed(Guid userId)
        {
            var result = await _loanService.AddBorrowedAsync(userId);
            return Ok(result);
        }
    }
}
