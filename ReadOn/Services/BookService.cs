using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using ReadOn.DbContexts;
using ReadOn.Dtos;
using ReadOn.Helpers;
using ReadOn.Models;
using ReadOn.Shared;

namespace ReadOn.Services
{
    public class BookService: IBookService
    {
        private readonly MyDbContext _dbContext;
        private readonly UserManager<ApplicationAccount> _userManager;

        public BookService(MyDbContext myDbContext, UserManager<ApplicationAccount> userManager)
        {
            _dbContext = myDbContext;
            _userManager = userManager;
        }
        
        public async Task<int> TotalBook()
        {
            int totalBook = await _dbContext.Books.CountAsync();
            return totalBook;
        }

        public async Task<List<BookDto>> Book(string? n)
        {
            var books = _dbContext.Books
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Fullname = b.Name,
                    Type = b.Type,
                    Language = b.Language,
                    Availability = (b.Available == 0) ? "Borrowed" : "Available"
                });
            if (!string.IsNullOrEmpty(n))
            {
                n = n.Trim();
                books = books.Where(b => b.Id.ToString().Contains(n) || b.Type.ToString().Contains(n));
            }
            return await books.ToListAsync();
        }

        public async Task<ViewBookDto> ViewBook(Guid id)
        {
            ViewBookDto book = await _dbContext.Books
                .Where(b => b.Id == id)
                .Select(b => new ViewBookDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Type = b.Type,
                    Language = b.Language,
                    Note = b.Note
                }).FirstOrDefaultAsync();
            return book;
        }

        public async Task<ApiResponse<Book>> AddBook(CreateUpdateBookDto value, Guid id)
        {
            try
            {
                var name = await _dbContext.Books.Where(u => u.Name == value.Name).FirstOrDefaultAsync();
                if (name != null)
                {
                    return new ApiResponse<Book>(false, "The name already exists.", null, 404);
                }

                var account = await _userManager.FindByIdAsync(id.ToString());

                if (account == null)
                {
                    return new ApiResponse<Book>(false, "Id is Incorrect.", null, 404);
                }

                bool isAdmin = await _userManager.IsInRoleAsync(account, AppRole.Admin);

                if (!isAdmin)
                {
                    return new ApiResponse<Book>(false, "User is not an Admin.", null, 403);
                }

                var book = new Book
                {
                    Id = Guid.NewGuid(),
                    Name = value.Name,
                    Type = value.Type,
                    Language = value.Language,
                    Total = value.Quantity,
                    Available = value.Quantity,
                    CreateAt = DateTime.Now,
                    Note = account.Firstname + " " + account.Lastname,
                    ApplicationAccountId = id
                };

                _dbContext.Books.Add(book);
                await _dbContext.SaveChangesAsync();

                return new ApiResponse<Book>(true, "Book created successfully.", book, 201);
            }
            catch (Exception ex)
            {
                return new ApiResponse<Book>(false, "An error occurred: " + ex.Message, null, 500);
            }
        }

        public async Task<ApiResponse<Book>> UpdateBook(CreateUpdateBookDto value, Guid id)
        {
            try
            {
                var name = await _dbContext.Books.Where(u => u.Name == value.Name).FirstOrDefaultAsync();
                if (name != null)
                {
                    return new ApiResponse<Book>(false, "The name already exists.", null, 404);
                }

                var book = await _dbContext.Books.FindAsync(id);
                if (book == null)
                    return new ApiResponse<Book>(false, "Book not found.", null, 404);

                book.Name = value.Name;
                book.Type = value.Type;
                book.Language = value.Language;
                int borrowed = book.Total - book.Available;
                book.Total = value.Quantity;
                book.Available = value.Quantity - borrowed;
                
                await _dbContext.SaveChangesAsync();

                return new ApiResponse<Book>(true, "Book updated successfully.", book, 201);
            }
            catch (Exception ex) 
            {
                return new ApiResponse<Book>(false, "An error occurred: " + ex.Message, null, 500);
            }
        }

        public async  Task<ApiResponse<bool>> DeleteBook(Guid id)
        {
            try
            {
                var book = await _dbContext.Books.FindAsync(id);
                if (book == null)
                    return new ApiResponse<bool>(false, "Book not found.", false, 404);

                _dbContext.Books.Remove(book);
                await _dbContext.SaveChangesAsync();
                return new ApiResponse<bool>(true, "Book deleted successfully.", true, 200);

            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, "An error occurred: " + ex.Message, false, 500);
            }
        }
    }
}
