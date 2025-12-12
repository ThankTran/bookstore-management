using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    /// <summary>
    /// thêm các phương thức cần thiết cho sách
    /// </summary>
    internal interface IBookRepository : IRepository<Book,string>
    {
        IEnumerable<Book> GetByCategory(BookCategory category); // tìm kiếm theo thể loại
        IEnumerable<Book> SearchByName(string keyword); // tìm kiếm theo tên
        IEnumerable<Book> GetByAuthor(string author);
        IEnumerable<Book> GetByPriceRange(decimal? minPrice, decimal? maxPrice);
    }
}
