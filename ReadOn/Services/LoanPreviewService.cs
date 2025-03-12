using Microsoft.EntityFrameworkCore;
using ReadOn.DbContexts;
using ReadOn.Dtos;

namespace ReadOn.Services
{
    public class LoanPreviewService: ILoanPreviewService
    {
        private readonly MyDbContext _dbContext;
        public LoanPreviewService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<LoanPreviewDto>> GetPreview(Guid id)
        {
            var loanPreviews = await _dbContext.LoanPreviews
                .Where(lp => lp.ApplicationAccountId == id)
                .Select(lp => new LoanPreviewDto
                {
                    ApplicationAccountId = lp.ApplicationAccountId,
                    BookId = lp.BookId,
                }).ToListAsync();

            return loanPreviews;
        }

        public async Task<LoanPreview> AddPreview(Guid id, Guid bookId)
        {
            var loanPreview = new LoanPreview
            {
                Id = Guid.NewGuid(),
                ApplicationAccountId = id,
                BookId = bookId,
            };

            _dbContext.LoanPreviews.Add(loanPreview);
            await _dbContext.SaveChangesAsync();

            return loanPreview;
        }

        public async Task<bool> RemovePreview(Guid id, Guid bookId)
        {
            var loanPreview = await _dbContext.LoanPreviews.FirstOrDefaultAsync(lp => lp.ApplicationAccountId == id && lp.BookId == bookId);

            _dbContext.LoanPreviews.Remove(loanPreview);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearPreview(Guid id)
        {
            var loanPreview = await _dbContext.LoanPreviews.Where(lp => lp.ApplicationAccountId == id).ToListAsync();
            
            _dbContext.LoanPreviews.RemoveRange(loanPreview);

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
