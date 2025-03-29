using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using ReadOn.DbContexts;
using ReadOn.Dtos;
using ReadOn.Helpers;
using ReadOn.Shared;
using System.Reflection.Metadata.Ecma335;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ReadOn.Services
{
    public class LoanService : ILoanService
    {
        private readonly MyDbContext _dbContext;
        private readonly UserManager<ApplicationAccount> _userManager;
        public LoanService(MyDbContext dbContext, UserManager<ApplicationAccount> userManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<LoanStatisticsDto> GetLoanStatisticsAsync()
        {
            int totalBorrowed = await _dbContext.LoansDetails.CountAsync();
            int totalReturned = await _dbContext.LoansDetails.CountAsync(ld => ld.Loan.Returndate != null);

            return new LoanStatisticsDto
            {
                TotalBorrowed = totalBorrowed - totalReturned,
                TotalReturned = totalReturned
            };
        }

        public async Task<LoanStatisticsDto> GetUserLoanStatisticsAsync(Guid id)
        {
            int totalBorrowed = await _dbContext.LoansDetails
                .Where( ld => _dbContext.Loans
                    .Where(l => l.ApplicationAccountId == id)
                    .Select(l=> l.Id)
                    .Contains(ld.LoanId)
                )
                .CountAsync();
            int totalReturned = await _dbContext.LoansDetails
                .Where(ld => _dbContext.Loans
                    .Where(l => l.ApplicationAccountId == id)
                    .Select(l => l.Id)
                    .Contains(ld.LoanId)
                )
                .CountAsync(ld => ld.Loan.Returndate != null);

            return new LoanStatisticsDto
            {
                TotalBorrowed = totalBorrowed - totalReturned,
                TotalReturned = totalReturned
            };
        }

        public async Task<List<ItemDto>> DBOverdueBorrowersAsync()
        {
            return await _dbContext.Loans
                .Where(l => l.Duedate < DateTime.Now && l.Returndate == null)
                .Select(l => new ItemDto
                {
                    Fullname = l.ApplicationAccount.Firstname + " "+ l.ApplicationAccount.Lastname,
                    Id = l.Id
                })
                .ToListAsync();
        }


        public async Task<List<BookInLoanDetailDto>> LoanDetailAsync(Guid id)
        {
            List<BookInLoanDetailDto> books = await _dbContext.LoansDetails
                .Where(ld => ld.LoanId == id)
                .Select(ld=> new BookInLoanDetailDto
                {
                    Id = ld.BookId,
                    Fullname = ld.Book.Name,
                    Type = ld.Book.Type,
                    Language = ld.Book.Language,
                })
                .ToListAsync();
            return books;
        }

        public async Task<List<BorrowedDto>> OverdueBorrowersAsync(string? n)
        {
            var overdueborrowersDtos = _dbContext.Loans
                .Where(l => l.Duedate < DateTime.Now && l.Returndate == null) 
                .Select(l => new BorrowedDto
                {
                    Id = l.Id,
                    UserId = l.ApplicationAccountId,
                    Amount = l.LoanDetails.Where(ld => ld.LoanId == l.Id).Sum(ld => ld.Quantity),
                    DueDate = l.Duedate,
                    Time = l.Borrowdate,
                });

            if (!string.IsNullOrEmpty(n))
            {
                n = n.Trim();
                overdueborrowersDtos = overdueborrowersDtos.Where(o => o.Id.ToString().Contains(n));
            }

            return await overdueborrowersDtos.ToListAsync();
        }

        public async Task<List<BorrowedDto>> BorrowedAsync(string? n)
        {
            var borrowedDtos = _dbContext.Loans
                .Where(l => l.Duedate >= DateTime.Now && l.Returndate == null)
                .Select(l => new BorrowedDto
                {
                    Id = l.Id,
                    UserId = l.ApplicationAccountId,
                    Amount = l.LoanDetails.Where(ld => ld.LoanId == l.Id).Sum(ld => ld.Quantity),
                    DueDate = l.Duedate,
                    Time = l.Borrowdate,
                });

            if (!string.IsNullOrEmpty(n))
            {
                n = n.Trim();
                borrowedDtos = borrowedDtos.Where(b => b.Id.ToString().Contains(n));
            }
                
            return await borrowedDtos.ToListAsync();
        }

        public async Task<List<BorrowedDto>> UserBorrowedAsync(Guid id, string? n)
        {
            var borrowedDtos = _dbContext.Loans
                .Where(l => l.Duedate >= DateTime.Now && l.Returndate == null && l.ApplicationAccountId == id)
                .Select(l => new BorrowedDto
                {
                    Id = l.Id,
                    UserId = l.ApplicationAccountId,
                    Amount = l.LoanDetails.Where(ld => ld.LoanId == l.Id).Sum(ld => ld.Quantity),
                    DueDate = l.Duedate,
                    Time = l.Borrowdate,
                });

            if (!string.IsNullOrEmpty(n))
            {
                n = n.Trim();
                borrowedDtos = borrowedDtos.Where(b => b.Id.ToString().Contains(n));
            }

            return await borrowedDtos.ToListAsync();
        }

        public async Task<List<BorrowedDto>> UserReturnedAsync(Guid id, string? n)
        {
            var borrowedDtos = _dbContext.Loans
                .Where(l => l.Returndate != null && l.ApplicationAccountId == id)
                .Select(l => new BorrowedDto
                {
                    Id = l.Id,
                    UserId = l.ApplicationAccountId,
                    Amount = l.LoanDetails.Where(ld => ld.LoanId == l.Id).Sum(ld => ld.Quantity),
                    DueDate = l.Duedate,
                    Time = l.Borrowdate,
                });

            if (!string.IsNullOrEmpty(n))
            {
                n = n.Trim();
                borrowedDtos = borrowedDtos.Where(b => b.Id.ToString().Contains(n));
            }

            return await borrowedDtos.ToListAsync();
        }

        public async Task<ApiResponse<Loan>> UserReturnedDetailAsync(Guid id)
        {
            var loan = await _dbContext.Loans.FindAsync(id);
            if (loan == null) {
                return new ApiResponse<Loan>(false, "Loan ID not found", null, 404);
            }
            
            loan.Returndate = DateTime.Now;
            loan.Status = LoanStatus.Return;

            foreach (var loanDetail in loan.LoanDetails)
            {
                var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == loanDetail.BookId);
                book.Available += 1;
            }

            await _dbContext.SaveChangesAsync();

            return new ApiResponse<Loan>(true, "Returned Successfully", loan, 201);
        }

        public async Task<ApiResponse<bool>> AddBorrowedAsync(Guid userId)
        {

            var loanPreviews = await _dbContext.LoanPreviews
                .Where(lp => lp.ApplicationAccountId == userId)
                .ToListAsync();

            if (!loanPreviews.Any())
            {
                return new ApiResponse<bool>(false, "No books in preview list", false, 400);
            }

            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                Borrowdate = DateTime.Now,
                Duedate = DateTime.Now.AddDays(14),
                Returndate = null,
                Status = LoanStatus.Borrow,
                Note = null,
                ApplicationAccountId = userId,
            };

            _dbContext.Loans.Add(loan);
            await _dbContext.SaveChangesAsync();

            foreach (var preview in loanPreviews)
            {
                var book = await _dbContext.Books.FindAsync(preview.BookId);
                if (book == null)
                {
                    return new ApiResponse<bool>(false, $"Book {preview.BookId} not found", false, 404);
                }
                if (book.Available <= 0)
                {
                    return new ApiResponse<bool>(false, $"Book {preview.BookId} is out of stock", false, 400);
                }

                var loandetail = new LoanDetail
                {
                    Id = Guid.NewGuid(),
                    Status = null,
                    Note = null,
                    LoanId = loan.Id,
                    BookId = preview.BookId,
                    Quantity = 1
                };

                _dbContext.LoansDetails.Add(loandetail);
                book.Available -= 1;
            }

            _dbContext.LoanPreviews.RemoveRange(loanPreviews);

            await _dbContext.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Loan created successfully.", true, 201);
        }
    }
}
