using AutoMapper;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Book.Requests;
using bookstore_Management.DTOs.Book.Responses;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bookstore_Management.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IImportBillDetailRepository _importBillDetailRepository;

        internal BookService(
            IBookRepository bookRepository,
            IStockRepository stockRepository,
            ISupplierRepository supplierRepository,
            IImportBillDetailRepository importBillDetailRepository)
        {
            _bookRepository = bookRepository;
            _stockRepository = stockRepository;
            _supplierRepository = supplierRepository;
            _importBillDetailRepository = importBillDetailRepository;
        }

        internal BookService()
        {
            var context = new BookstoreDbContext();
            _bookRepository = new BookRepository(context);
        }
        // Hãy đảm bảo bạn đã gán giá trị cho repository
        internal BookService(IBookRepository bookRepo)
        {
            _bookRepository = bookRepo;
        }

        // ==================================================================
        // ---------------------- THÊM DỮ LIỆU ------------------------------
        // ==================================================================
        public Result<string> CreateBook(CreateBookRequestDto dto)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<string>.Fail("Tên sách không được trống");
                if (string.IsNullOrWhiteSpace(dto.Author))
                    return Result<string>.Fail("Tác giả không được trống");

                if (!string.IsNullOrWhiteSpace(dto.SupplierId))
                {
                    var sup = _supplierRepository.GetById(dto.SupplierId);
                    if (sup == null || sup.DeletedDate != null)
                        return Result<string>.Fail("Nhà cung cấp không tồn tại");
                }

                // Generate Book ID -- nếu không có bookId autoGen
                var bookId = string.IsNullOrWhiteSpace(dto.Id) ? GenerateBookId() : dto.Id.Trim();
                if (_bookRepository.Exists(b => b.BookId == bookId))
                    return Result<string>.Fail("Mã sách đã tồn tại");

                // Auto set SalePrice nếu không có
                var salePrice = dto.SalePrice;

                // Create book
                var book = new Book
                {
                    BookId = bookId,
                    Name = dto.Name.Trim(),
                    Author = dto.Author?.Trim(),
                    SupplierId = string.IsNullOrWhiteSpace(dto.SupplierId) ? null : dto.SupplierId.Trim(),
                    Category = dto.Category,
                    SalePrice = salePrice,
                    CreatedDate = DateTime.Now
                };

                _bookRepository.Add(book);
                _bookRepository.SaveChanges();

                return Result<string>.Success(bookId, "Thêm sách thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- SỬA DỮ LIỆU ------------------------------
        // ==================================================================
        public Result UpdateBook(string bookId, UpdateBookRequestDto dto)
        {
            try
            {
                var book = _bookRepository.GetById(bookId);
                if (book == null || book.DeletedDate != null)
                    return Result.Fail("Sách không tồn tại");

                if (!string.IsNullOrWhiteSpace(dto.Name))
                    book.Name = dto.Name.Trim();
                if (!string.IsNullOrWhiteSpace(dto.Author))
                    book.Author = dto.Author.Trim();
                if (dto.Category.HasValue)
                    book.Category = dto.Category.Value;
                if (dto.SalePrice.HasValue)
                    book.SalePrice = dto.SalePrice.Value;
                if (!string.IsNullOrWhiteSpace(dto.SupplierId))
                {
                    var sup = _supplierRepository.GetById(dto.SupplierId);
                    if (sup == null || sup.DeletedDate != null)
                        return Result.Fail("Nhà cung cấp không tồn tại");
                    book.SupplierId = dto.SupplierId.Trim();
                }
                book.UpdatedDate = DateTime.Now;

                _bookRepository.Update(book);
                _bookRepository.SaveChanges();

                return Result.Success("Cập nhật sách thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ---------------------- XÓA DỮ LIỆU -------------------------------
        // ==================================================================
        public Result DeleteBook(string bookId)
        {
            try
            {
                var book = _bookRepository.GetById(bookId);
                if (book == null || book.DeletedDate != null)
                    return Result.Fail("Sách không tồn tại");

                // Soft delete
                book.DeletedDate = DateTime.Now;
                _bookRepository.Update(book);
                _bookRepository.SaveChanges();

                return Result.Success("Xóa sách thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- LẤY DỮ LIỆU ------------------------------
        // ==================================================================


        public Result<BookDetailResponseDto> GetBookById(string bookId)
        {
            if (string.IsNullOrWhiteSpace(bookId))
                return Result<BookDetailResponseDto>.Fail("Mã sách không hợp lệ");

            try
            {
                var book = _bookRepository.GetById(bookId);

                if (book == null || book.DeletedDate != null)
                    return Result<BookDetailResponseDto>.Fail("Sách không tồn tại");

                var dto = new BookDetailResponseDto(
                    book.BookId,
                    book.Name,
                    book.Author,
                    book.Category,
                    book.SalePrice,
                    book.Supplier?.Name
                );

                return Result<BookDetailResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<BookDetailResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<decimal> GetImportPrice(string bookId)
        {
            try
            {
                var importPrice = _importBillDetailRepository.GetImportPriceByBookId(bookId);
                return Result<decimal>.Success(importPrice);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<BookDetailResponseDto>> GetAllBooks()
        {
            try
            {
                var books = _bookRepository.GetAll()
                    .Where(b => b.DeletedDate == null)
                    .Select(book => new BookDetailResponseDto(
                        book.BookId,
                        book.Name,
                        book.Author,
                        book.Category,
                        book.SalePrice,
                        book.Supplier?.Name
                    ));

                return Result<IEnumerable<BookDetailResponseDto>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BookDetailResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<BookDetailResponseDto>> SearchByName(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return Result<IEnumerable<BookDetailResponseDto>>.Success(new List<BookDetailResponseDto>());

                var books = _bookRepository.SearchByName(keyword)
                    .Where(b => b.DeletedDate == null)
                    .Select(book => new BookDetailResponseDto(
                        book.BookId,
                        book.Name,
                        book.Author,
                        book.Category,
                        book.SalePrice,
                        book.Supplier?.Name
                    ));

                return Result<IEnumerable<BookDetailResponseDto>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BookDetailResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<BookDetailResponseDto>> GetByCategory(BookCategory category)
        {
            try
            {
                var books = _bookRepository.GetByCategory(category)
                    .Where(b => b.DeletedDate == null)
                    .Select(book => new BookDetailResponseDto(
                        book.BookId,
                        book.Name,
                        book.Author,
                        book.Category,
                        book.SalePrice,
                        book.Supplier?.Name
                    ));

                return Result<IEnumerable<BookDetailResponseDto>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BookDetailResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<BookDetailResponseDto>> GetByAuthor(string author)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(author))
                    return Result<IEnumerable<BookDetailResponseDto>>.Success(new List<BookDetailResponseDto>());

                var books = _bookRepository.GetByAuthor(author)
                    .Where(b => b.DeletedDate == null)
                    .Select(book => new BookDetailResponseDto(
                        book.BookId,
                        book.Name,
                        book.Author,
                        book.Category,
                        book.SalePrice,
                        book.Supplier?.Name
                    ));

                return Result<IEnumerable<BookDetailResponseDto>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BookDetailResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<BookDetailResponseDto>> GetByPriceRange(decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                var books = _bookRepository.GetByPriceRange(minPrice, maxPrice)
                    .Where(b => b.DeletedDate == null)
                    .Select(book => new BookDetailResponseDto(
                        book.BookId,
                        book.Name,
                        book.Author,
                        book.Category,
                        book.SalePrice,
                        book.Supplier?.Name
                    ));

                return Result<IEnumerable<BookDetailResponseDto>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BookDetailResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<BookDetailResponseDto>> GetBySupplierName(string supplierName)
        {
            try
            {
                var books = _bookRepository.Find(b => b.Supplier.Name == supplierName)
                    .Select(book => new BookDetailResponseDto(
                    book.BookId,
                    book.Name,
                    book.Author,
                    book.Category,
                    book.SalePrice,
                    book.Supplier?.Name
                ));

                return Result<IEnumerable<BookDetailResponseDto>>.Success(books);
            }
            catch (Exception e)
            {
                return Result<IEnumerable<BookDetailResponseDto>>.Fail(e.Message);
            }
        }

        public Result<IEnumerable<BookDetailResponseDto>> GetLowStockBooks(int minStock = 5)
        {
            try
            {
                var stocks = _stockRepository.GetLowStock(minStock);
                var bookIds = stocks.Select(s => s.BookId).Distinct().ToList();

                var books = _bookRepository.Find(b => bookIds.Contains(b.BookId) && b.DeletedDate == null)
                    .Select(book => new BookDetailResponseDto(
                    book.BookId,
                    book.Name,
                    book.Author,
                    book.Category,
                    book.SalePrice,
                    book.Supplier?.Name
                ));

                return Result<IEnumerable<BookDetailResponseDto>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BookDetailResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<BookDetailResponseDto>> GetOutOfStockBooks()
        {
            try
            {
                var outOfStocks = _stockRepository.Find(s => s.StockQuantity == 0);
                var bookIds = outOfStocks.Select(s => s.BookId).Distinct().ToList();

                var books = _bookRepository.Find(b => bookIds.Contains(b.BookId) && b.DeletedDate == null)
                    .Select(book => new BookDetailResponseDto(
                    book.BookId,
                    book.Name,
                    book.Author,
                    book.Category,
                    book.SalePrice,
                    book.Supplier?.Name
                ));

                return Result<IEnumerable<BookDetailResponseDto>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BookDetailResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- LIST VIEW METHODS -------------------------
        // ==================================================================
        public Result<IEnumerable<BookListResponseDto>> GetBookList()
        {
            try
            {
                // Get all active books (already filtered by DeletedDate in repository)
                var books = _bookRepository.GetAllForListView().ToList();

                if (!books.Any())
                    return Result<IEnumerable<BookListResponseDto>>.Success(new List<BookListResponseDto>());

                // Get import prices for all books in one batch query
                var bookIds = books.Select(b => b.BookId).ToList();
                var importPrices = _importBillDetailRepository.GetLatestImportPricesByBookIds(bookIds);

                // Map to DTOs with Index (STT) generation
                var result = books.Select((book, index) => new BookListResponseDto
                {
                    Index = index + 1, // STT starts from 1
                    BookId = book.BookId,
                    Name = book.Name,
                    SupplierId = book.SupplierId,
                    Category = book.Category,
                    SalePrice = book.SalePrice,
                    ImportPrice = importPrices.ContainsKey(book.BookId) ? importPrices[book.BookId] : null
                }).ToList();

                return Result<IEnumerable<BookListResponseDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BookListResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- HÀM HELPER --------------------------------
        // ==================================================================
        private string GenerateBookId()
        {
            var lastBook = _bookRepository.GetAll()
                .OrderByDescending(b => b.BookId)
                .FirstOrDefault();

            if (lastBook == null || !lastBook.BookId.StartsWith("S"))
                return "S00001";

            var lastNumber = int.Parse(lastBook.BookId.Substring(1));
            return $"S{(lastNumber + 1):D5}";
        }
    }
}