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
        public IEnumerable<Book> GetByCategory(BookCategory category)
        {
            return Find(b => b.Category == category && b.DeletedDate == null);
        }

        public IEnumerable<Book> SearchByName(string keyword)
        {
            return Find(b => b.Name.Contains(keyword) && b.DeletedDate == null);
        }
        
        public IEnumerable<Book> GetByAuthor(string author)
        {
            return Find(b => b.Author.Contains(author) && b.DeletedDate == null);
        }

        public IEnumerable<Book> GetByPriceRange(decimal? minPrice, decimal? maxPrice)
        {
            return Find(b =>
                b.DeletedDate == null &&
                (!minPrice.HasValue || (b.SalePrice.HasValue && b.SalePrice.Value >= minPrice.Value)) &&
                (!maxPrice.HasValue || (b.SalePrice.HasValue && b.SalePrice.Value <= maxPrice.Value)));
        }

        /// <summary>
        /// Gets all active (non-deleted) books for list view
        /// Filters by DeletedDate == null
        /// </summary>
        public IEnumerable<Book> GetAllForListView()
        {
            return Find(b => b.DeletedDate == null);
        }
    }
}
