using System.Collections.Generic;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Book.Requests;
using bookstore_Management.DTOs.Book.Responses;

namespace bookstore_Management.Services.Interfaces
{
    public interface IBookService
    {
        Task<Result<string>> CreateBookAsync(CreateBookRequestDto dto);
        Task<Result> UpdateBookAsync(string bookId, UpdateBookRequestDto dto);
        Task<Result> DeleteBookAsync(string bookId);

        Task<Result<BookDetailResponseDto>> GetBookByIdAsync(string bookId);
        Task<Result<decimal>> GetImportPriceAsync(string bookId);

        Task<Result<IEnumerable<BookDetailResponseDto>>> GetAllBooksAsync();
        Task<Result<IEnumerable<BookDetailResponseDto>>> SearchByNameAsync(string keyword);
        Task<Result<IEnumerable<BookDetailResponseDto>>> GetByCategoryAsync(BookCategory category);
        Task<Result<IEnumerable<BookDetailResponseDto>>> GetByAuthorAsync(string author);
        Task<Result<IEnumerable<BookDetailResponseDto>>> GetByPriceRangeAsync(decimal? minPrice, decimal? maxPrice);
        Task<Result<IEnumerable<BookDetailResponseDto>>> GetByPublisherNameAsync(string publisherName);

        Task<Result<IEnumerable<BookDetailResponseDto>>> GetLowStockBooksAsync(int minStock = 5);
        Task<Result<IEnumerable<BookDetailResponseDto>>> GetOutOfStockBooksAsync();

        Task<Result<IEnumerable<BookListResponseDto>>> GetBookListAsync();
    }
}