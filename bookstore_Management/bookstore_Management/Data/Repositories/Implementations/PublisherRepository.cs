using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class PublisherRepository : Repository<Publisher, string>, IPublisherRepository
    {
        public PublisherRepository(BookstoreDbContext context) : base(context) { }

        public IQueryable<Publisher> SearchByName(string keyword)
        {
            return Query(p => p.Name.Contains(keyword) && p.DeletedDate == null);
        }

        public async Task<string> GetNameByPublisherIdAsync(string publisherId)
        {
            return await DbSet
                .Where(s => s.Id == publisherId && s.DeletedDate == null)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();
        }

        public async Task<Publisher> GetByPhoneAsync(string phone)
        {
            return await DbSet
                .Where(s => s.Phone == phone && s.DeletedDate == null)
                .FirstOrDefaultAsync();
        }

        public async Task<Publisher> GetByEmailAsync(string email)
        {
            return await DbSet
                .Where(s => s.Email == email && s.DeletedDate == null)
                .FirstOrDefaultAsync();
        }
    }
}