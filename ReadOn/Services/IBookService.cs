using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ReadOn.DbContexts;
using ReadOn.Dtos;
using ReadOn.Shared;

namespace ReadOn.Services
{
    public interface IBookService
    {
        Task<int> TotalBook();
        Task<List<BookDto>> Book(string? n);
        Task<ApiResponse<Book>> AddBook(CreateUpdateBookDto value, Guid id);
        Task<ApiResponse<Book>> UpdateBook(CreateUpdateBookDto value, Guid id);
        Task<ApiResponse<bool>> DeleteBook(Guid id);
        Task<ViewBookDto> ViewBook(Guid id);
    }
}
