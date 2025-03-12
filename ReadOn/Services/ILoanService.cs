using ReadOn.DbContexts;
using ReadOn.Dtos;
using ReadOn.Shared;

namespace ReadOn.Services
{
    public interface ILoanService
    {
        Task<LoanStatisticsDto> GetLoanStatisticsAsync();
        Task<LoanStatisticsDto> GetUserLoanStatisticsAsync(Guid id);
        Task<List<ItemDto>> DBOverdueBorrowersAsync();
        Task<List<BorrowedDto>> OverdueBorrowersAsync(string? n);
        Task<List<BookInLoanDetailDto>> LoanDetailAsync(Guid id);
        Task<List<BorrowedDto>> BorrowedAsync(string? n);
        Task<List<BorrowedDto>> UserBorrowedAsync(Guid id, string? n);
        Task<List<BorrowedDto>> UserReturnedAsync(Guid id, string? n);
        Task<ApiResponse<Loan>> UserReturnedDetailAsync(Guid id);
        Task<ApiResponse<bool>> AddBorrowedAsync(Guid userId);
    }
}
