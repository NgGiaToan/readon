using ReadOn.DbContexts;
using ReadOn.Dtos;

namespace ReadOn.Services
{
    public interface ILoanPreviewService
    {
        Task<List<LoanPreviewDto>> GetPreview(Guid id);
        Task<LoanPreview> AddPreview(Guid id, Guid bookId);
        Task<bool> RemovePreview(Guid id, Guid bookId);
        Task<bool> ClearPreview(Guid id);
    }
}
