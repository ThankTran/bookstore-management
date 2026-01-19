using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    /// <summary>
    /// thêm các phương thức cần thiết cho sách
    /// </summary>
    internal interface IBookRepository : IRepository<Book,string>
    {
        Task<IEnumerable<Book>> GetByCategoryAsync(BookCategory category); // tìm kiếm theo thể loại
        IQueryable<Book> SearchByName(string keyword);
        Task<IEnumerable<Book>> GetByAuthorAsync(string author);
        Task<IEnumerable<Book>> GetByPriceRangeAsync(decimal? minPrice, decimal? maxPrice);
        
        /// <summary>
        /// Gets all active (non-deleted) books for list view
        /// Filters by DeletedDate == null
        /// </summary>
        Task<IEnumerable<Book>> GetAllForListViewAsync();
    }
}
