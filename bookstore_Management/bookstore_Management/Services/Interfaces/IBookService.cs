using System;
using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface IBookService
    {
        // CRUD
        Result<string> CreateBook(BookCreateDto dto);
        Result UpdateBook(string bookId, BookUpdateDto dto);
        Result DeleteBook(string bookId);
        Result<Book> GetBookById(string bookId);
        Result<IEnumerable<Book>> GetAllBooks();
        // Tìm kiếm & Lọc
        Result<IEnumerable<Book>> SearchByName(string keyword);
        Result<IEnumerable<Book>> GetByCategory(BookCategory category);
        Result<IEnumerable<Book>> GetByAuthor(string author);
        Result<IEnumerable<Book>> GetByPriceRange(decimal? minPrice, decimal? maxPrice);

        // Quản lý giá
        Result UpdateSalePrice(string bookId, decimal newSalePrice);
    
        // Quản lý tồn kho (gọi đến StockService)
        Result<IEnumerable<Book>> GetLowStockBooks(int minStock = 5);
        Result<IEnumerable<Book>> GetOutOfStockBooks();
    }
}