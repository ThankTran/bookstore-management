using System;
using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Book.Requests;
using bookstore_Management.DTOs.Book.Responses;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface IBookService
    {
        // CRUD
        Result<string> CreateBook(CreateBookRequestDto dto);
        Result UpdateBook(string bookId, UpdateBookRequestDto dto);
        Result DeleteBook(string bookId);
        Result<BookDetailResponseDto> GetBookById(string bookId);
        Result<IEnumerable<BookDetailResponseDto>> GetAllBooks();
        
        
        // Tìm kiếm & Lọc
        Result<IEnumerable<BookDetailResponseDto>> SearchByName(string keyword);
        Result<decimal> GetImportPrice(string bookId);
        Result<IEnumerable<BookDetailResponseDto>> GetByCategory(BookCategory category);
        Result<IEnumerable<BookDetailResponseDto>> GetByAuthor(string author);
        
        Result<IEnumerable<BookDetailResponseDto>> GetBySupplierName(string supplierName);
        Result<IEnumerable<BookDetailResponseDto>> GetByPriceRange(decimal? minPrice, decimal? maxPrice);
        
        
        
        // Quản lý tồn kho (gọi đến StockService)
        Result<IEnumerable<BookDetailResponseDto>> GetLowStockBooks(int minStock = 5);
        Result<IEnumerable<BookDetailResponseDto>> GetOutOfStockBooks();

        // ListviewItem (optional)
        Result<IEnumerable<BookListResponseDto>> GetBookList();
    }
}