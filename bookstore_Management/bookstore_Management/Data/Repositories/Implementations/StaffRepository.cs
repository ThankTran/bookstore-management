using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;
using System.Data.Entity;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class StaffRepository : Repository<Staff, string>, IStaffRepository
    {
        public StaffRepository(BookstoreDbContext context) : base(context) { }

        public async Task<IEnumerable<Staff>> GetByRoleAsync(UserRole userRole)
        {
            return await DbSet
                .Where(s => s.UserRole == userRole && s.DeletedDate == null)
                .ToListAsync();
        }

        public IQueryable<Staff> SearchByName(string keyword)
        {
            return Query(s => s.Name.Contains(keyword) && s.DeletedDate == null);
        }
    }
}