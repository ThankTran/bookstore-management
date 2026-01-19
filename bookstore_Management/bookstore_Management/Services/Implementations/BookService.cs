using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Book.Requests;
using bookstore_Management.DTOs.Book.Responses;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace bookstore_Management.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        internal BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==============================================================
        // CREATE
        // ==============================================================
        public async Task<Result<string>> CreateBookAsync(CreateBookRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<string>.Fail("Tên sách không được trống");

            if (string.IsNullOrWhiteSpace(dto.Author))
                return Result<string>.Fail("Tác giả không được trống");

            if (dto.SalePrice <= 0)
                return Result<string>.Fail("Giá bán phải lớn hơn 0");

            Publisher publisher = null;
            if (!string.IsNullOrWhiteSpace(dto.PublisherName))
            {
                publisher = await _unitOfWork.Publishers.Query()
                    .Where(x => x.DeletedDate == null &&
                                x.Name.ToLower() == dto.PublisherName.Trim().ToLower())
                    .FirstOrDefaultAsync();
            }

            var bookId = await GenerateBookIdAsync();

            if (await _unitOfWork.Books.AnyAsync(b => b.BookId == bookId))
                return Result<string>.Fail($"Mã sách {bookId} đã tồn tại");

            var book = new Book
            {
                BookId = bookId,
                Name = dto.Name.Trim(),
                Author = dto.Author.Trim(),
                PublisherId = publisher?.Id,
                Category = dto.Category,
                SalePrice = dto.SalePrice,
                Stock = 0,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(bookId);
        }

        // ==============================================================
        // UPDATE
        // ==============================================================
        public async Task<Result> UpdateBookAsync(string bookId, UpdateBookRequestDto dto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(bookId);

            if (book == null || book.DeletedDate != null)
                return Result.Fail("Sách không tồn tại");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                book.Name = dto.Name.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Author))
                book.Author = dto.Author.Trim();

            if (dto.Category.HasValue)
                book.Category = dto.Category.Value;

            if (dto.SalePrice.HasValue)
            {
                if (dto.SalePrice.Value <= 0)
                    return Result.Fail("Giá phải lớn hơn 0");

                book.SalePrice = dto.SalePrice.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.PublisherName))
            {
                var publisher = await _unitOfWork.Publishers
                    .SearchByName(dto.PublisherName)
                    .Where(p => p.DeletedDate == null)
                    .FirstOrDefaultAsync();

                if (publisher == null)
                    return Result.Fail("Nhà xuất bản không hợp lệ");

                book.PublisherId = publisher.Id;
            }

            book.UpdatedDate = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Cập nhật thành công");
        }

        // ==============================================================
        // DELETE
        // ==============================================================
        public async Task<Result> DeleteBookAsync(string bookId)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(bookId);

            if (book == null || book.DeletedDate != null)
                return Result.Fail("Sách không tồn tại");

            book.DeletedDate = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Đã xóa");
        }

        // ==============================================================
        // GET BY ID - Optimized với projection
        // ==============================================================
        public async Task<Result<BookDetailResponseDto>> GetBookByIdAsync(string bookId)
        {
            var result = await _unitOfWork.Books
                .Query(b => b.BookId == bookId && b.DeletedDate == null)
                .Select(b => new BookDetailResponseDto
                {
                    BookId = b.BookId,
                    Name = b.Name,
                    Author = b.Author,
                    Category = b.Category,
                    SalePrice = b.SalePrice,
                    StockQuantity = b.Stock,
                    ImportPrice = _unitOfWork.ImportBillDetails
                        .Query(x => x.BookId == b.BookId)
                        .Select(x => x.ImportPrice)
                        .FirstOrDefault(),
                    PublisherName = _unitOfWork.Publishers
                        .Query(x => x.Id == b.PublisherId)
                        .Select(x => x.Name)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (result == null)
                return Result<BookDetailResponseDto>.Fail("Không tìm thấy");

            return Result<BookDetailResponseDto>.Success(result);
        }

        // ==============================================================
        // IMPORT PRICE
        // ==============================================================
        public async Task<Result<decimal>> GetImportPriceAsync(string bookId)
        {
            var price = await _unitOfWork.ImportBillDetails.GetImportPriceByBookIdAsync(bookId);
            return Result<decimal>.Success(price);
        }

        // ==============================================================
        // GET ALL - Optimized với projection
        // ==============================================================
        public async Task<Result<IEnumerable<BookDetailResponseDto>>> GetAllBooksAsync()
        {
            var books = await _unitOfWork.Books
                .Query(b => b.DeletedDate == null)
                .Select(b => new BookDetailResponseDto
                {
                    BookId = b.BookId,
                    Name = b.Name,
                    Author = b.Author,
                    Category = b.Category,
                    SalePrice = b.SalePrice,
                    StockQuantity = b.Stock,
                    ImportPrice = _unitOfWork.ImportBillDetails
                        .Query(x => x.BookId == b.BookId)
                        .Select(x => x.ImportPrice)
                        .FirstOrDefault(),
                    PublisherName = _unitOfWork.Publishers
                        .Query(x => x.Id == b.PublisherId)
                        .Select(x => x.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Result<IEnumerable<BookDetailResponseDto>>.Success(books);
        }

        // ==============================================================
        // SEARCH - Optimized với GroupJoin
        // ==============================================================
        public async Task<Result<IEnumerable<BookDetailResponseDto>>> SearchByNameAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Result<IEnumerable<BookDetailResponseDto>>.Success(new List<BookDetailResponseDto>());

            var result = await _unitOfWork.Books
                .SearchByName(keyword)
                .Where(b => b.DeletedDate == null)
                .GroupJoin(
                    _unitOfWork.ImportBillDetails.Query(),
                    book => book.BookId,
                    detail => detail.BookId,
                    (book, details) => new { book, details }
                )
                .GroupJoin(
                    _unitOfWork.Publishers.Query(),
                    x => x.book.PublisherId,
                    pub => pub.Id,
                    (x, publishers) => new { x.book, x.details, publishers }
                )
                .Select(x => new BookDetailResponseDto
                {
                    BookId = x.book.BookId,
                    Name = x.book.Name,
                    Author = x.book.Author,
                    Category = x.book.Category,
                    SalePrice = x.book.SalePrice,
                    StockQuantity = x.book.Stock,
                    ImportPrice = x.details.Select(d => d.ImportPrice).FirstOrDefault(),
                    PublisherName = x.publishers.Select(p => p.Name).FirstOrDefault()
                })
                .ToListAsync();

            return Result<IEnumerable<BookDetailResponseDto>>.Success(result);
        }

        // ==============================================================
        // CATEGORY - Optimized với batch loading
        // ==============================================================
        public async Task<Result<IEnumerable<BookDetailResponseDto>>> GetByCategoryAsync(BookCategory category)
        {
            var books = await _unitOfWork.Books
                .Query(b => b.DeletedDate == null && b.Category == category)
                .ToListAsync();

            if (!books.Any())
                return Result<IEnumerable<BookDetailResponseDto>>.Success(new List<BookDetailResponseDto>());

            return Result<IEnumerable<BookDetailResponseDto>>.Success(await MapListAsync(books));
        }

        // ==============================================================
        // AUTHOR
        // ==============================================================
        public async Task<Result<IEnumerable<BookDetailResponseDto>>> GetByAuthorAsync(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
                return Result<IEnumerable<BookDetailResponseDto>>.Success(Array.Empty<BookDetailResponseDto>());

            var books = await _unitOfWork.Books.GetByAuthorAsync(author);
            return Result<IEnumerable<BookDetailResponseDto>>.Success(await MapListAsync(books));
        }

        // ==============================================================
        // PRICE RANGE
        // ==============================================================
        public async Task<Result<IEnumerable<BookDetailResponseDto>>> GetByPriceRangeAsync(decimal? min, decimal? max)
        {
            var books = await _unitOfWork.Books.GetByPriceRangeAsync(min, max);
            return Result<IEnumerable<BookDetailResponseDto>>.Success(await MapListAsync(books));
        }

        // ==============================================================
        // PUBLISHER
        // ==============================================================
        public async Task<Result<IEnumerable<BookDetailResponseDto>>> GetByPublisherNameAsync(string publisherName)
        {
            var books = await _unitOfWork.Books.FindAsync(
                b => b.Publisher.Name == publisherName && b.DeletedDate == null
            );
            return Result<IEnumerable<BookDetailResponseDto>>.Success(await MapListAsync(books));
        }

        // ==============================================================
        // LOW STOCK
        // ==============================================================
        public async Task<Result<IEnumerable<BookDetailResponseDto>>> GetLowStockBooksAsync(int minStock = 5)
        {
            var books = await _unitOfWork.Books.FindAsync(
                b => b.Stock <= minStock && b.DeletedDate == null
            );
            return Result<IEnumerable<BookDetailResponseDto>>.Success(await MapListAsync(books));
        }

        // ==============================================================
        // OUT OF STOCK
        // ==============================================================
        public async Task<Result<IEnumerable<BookDetailResponseDto>>> GetOutOfStockBooksAsync()
        {
            var books = await _unitOfWork.Books.FindAsync(
                b => b.Stock == 0 && b.DeletedDate == null
            );
            return Result<IEnumerable<BookDetailResponseDto>>.Success(await MapListAsync(books));
        }

        // ==============================================================
        // LIST VIEW
        // ==============================================================
        public async Task<Result<IEnumerable<BookListResponseDto>>> GetBookListAsync()
        {
            var books = await _unitOfWork.Books.GetAllForListViewAsync();
            var bookIds = books.Select(x => x.BookId).ToList();
            var importPrices = await _unitOfWork.ImportBillDetails
                .GetLatestImportPricesByBookIdsAsync(bookIds);

            var result = books.Select((b, i) => new BookListResponseDto
            {
                Index = i + 1,
                BookId = b.BookId,
                Name = b.Name,
                PublisherId = b.PublisherId,
                Category = b.Category,
                SalePrice = b.SalePrice,
                ImportPrice = importPrices.TryGetValue(b.BookId, out var p) ? p : null
            }).ToList();

            return Result<IEnumerable<BookListResponseDto>>.Success(result);
        }

        // ==============================================================
        // HELPER - Optimized ID generation
        // ==============================================================
        private async Task<string> GenerateBookIdAsync()
        {
            var lastBook = await _unitOfWork.Books.Query()
                .OrderByDescending(b => b.BookId)
                .Select(b => b.BookId)
                .FirstOrDefaultAsync();

            int number = 0;
            if (!string.IsNullOrEmpty(lastBook))
            {
                number = int.Parse(lastBook.Substring(1));
            }
            number++;

            return $"S{number:D5}";
        }

        // ==============================================================
        // HELPER - Optimized batch mapping
        // ==============================================================
        private async Task<IEnumerable<BookDetailResponseDto>> MapListAsync(IEnumerable<Book> books)
        {
            var bookList = books.ToList();
            if (bookList.Count == 0)
                return Array.Empty<BookDetailResponseDto>();

            var bookIds = bookList.Select(b => b.BookId).ToList();
            var publisherIds = bookList.Where(b => b.PublisherId != null)
                .Select(b => b.PublisherId).Distinct().ToList();

            // Batch load import prices
            var importPrices = await _unitOfWork.ImportBillDetails
                .Query(x => bookIds.Contains(x.BookId))
                .GroupBy(x => x.BookId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.ImportPrice).FirstOrDefault());

            // Batch load publisher names
            var publisherNames = publisherIds.Any()
                ? await _unitOfWork.Publishers
                    .Query(x => publisherIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.Name)
                : new Dictionary<string, string>();

            return bookList.Select(b => new BookDetailResponseDto
            {
                BookId = b.BookId,
                Name = b.Name,
                Author = b.Author,
                Category = b.Category,
                SalePrice = b.SalePrice,
                StockQuantity = b.Stock,
                ImportPrice = importPrices.TryGetValue(b.BookId, out var price) ? price : 0,
                PublisherName = b.PublisherId != null && publisherNames.TryGetValue(b.PublisherId, out var pname) 
                    ? pname : null
            });
        }
    }
}