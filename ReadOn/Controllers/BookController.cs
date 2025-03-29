using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ReadOn.DbContexts;
using ReadOn.Dtos;
using ReadOn.Helpers;
using ReadOn.Services;
using ReadOn.Shared;

namespace ReadOn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpGet("dashboard-totalbook")]
        public async Task<IActionResult> GetTotal()
        {
            int totalBook = await _bookService.TotalBook();
            return Ok(totalBook);
        }

        [Authorize]
        [HttpGet("books")]
        public async Task<IActionResult> BookManagement(string? n)
        {
            List<BookDto> bookDtos = await _bookService.Book(n);
            return Ok(bookDtos);
        }

        [Authorize]
        [HttpGet("view-book")]
        public async Task<IActionResult> ViewBook(Guid id)
        {
            ViewBookDto book = await _bookService.ViewBook(id);
            return Ok(book);
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpPost("create-book")]
        public async Task<IActionResult> CreateBook(CreateUpdateBookDto value,Guid id)
        {
            ApiResponse<Book> book = await _bookService.AddBook(value, id);
            return StatusCode(book.StatusCode, new { success = book.Success, message = book.Message, data = book.Data });
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpPut("update-book")]
        public async Task<IActionResult> UpdateBook(CreateUpdateBookDto value, Guid id)
        {
            ApiResponse<Book> book = await _bookService.UpdateBook(value, id);
            return StatusCode(book.StatusCode, new {success = book.Success, message = book.Message, data = book.Data });
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpDelete("delete-book")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            ApiResponse<bool> book = await _bookService.DeleteBook(id);
            return StatusCode(book.StatusCode, new { success = book.Success, message = book.Message, data = book.Data });
        }
    }
}
