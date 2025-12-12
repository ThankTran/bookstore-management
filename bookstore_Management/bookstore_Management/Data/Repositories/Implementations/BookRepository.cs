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
            return Find(b => b.Category == category);
        }

        public IEnumerable<Book> SearchByName(string keyword)
        {
            return Find(b => b.Name.Contains(keyword));
        }
        
        public IEnumerable<Book> GetByAuthor(string author)
        {
            return Find(b => b.Author.Contains(author));
        }

        public IEnumerable<Book> GetByPriceRange(decimal? minPrice, decimal? maxPrice)
        {
            return Find(b =>
                (!minPrice.HasValue || (b.SalePrice.HasValue && b.SalePrice.Value >= minPrice.Value)) &&
                (!maxPrice.HasValue || (b.SalePrice.HasValue && b.SalePrice.Value <= maxPrice.Value)));
        }
    }
}
