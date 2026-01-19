using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class BookRepository : Repository<Book,string>,IBookRepository
    {
        public BookRepository(BookstoreDbContext context) : base(context) { }
        public  Task<IEnumerable<Book>> GetByCategoryAsync(BookCategory category)
        {
            return FindAsync(b => b.Category == category && b.DeletedDate == null);
        }

        public IQueryable<Book> SearchByName(string keyword)
        {
            return Query(b => b.Name.Contains(keyword) && b.DeletedDate == null);
        }
        
        public  Task<IEnumerable<Book>> GetByAuthorAsync(string author)
        {
            return FindAsync(b => b.Author.Contains(author) && b.DeletedDate == null);
        }

        public  Task<IEnumerable<Book>> GetByPriceRangeAsync(decimal? minPrice, decimal? maxPrice)
        {
            return FindAsync(b =>
                b.DeletedDate == null &&
                (!minPrice.HasValue || (b.SalePrice.HasValue && b.SalePrice.Value >= minPrice.Value)) &&
                (!maxPrice.HasValue || (b.SalePrice.HasValue && b.SalePrice.Value <= maxPrice.Value)));
        }

        /// <summary>
        /// Gets all active (non-deleted) books for list view
        /// Filters by DeletedDate == null
        /// </summary>
        public Task<IEnumerable<Book>> GetAllForListViewAsync()
        {
            return FindAsync(b => b.DeletedDate == null);
        }
    }
}
