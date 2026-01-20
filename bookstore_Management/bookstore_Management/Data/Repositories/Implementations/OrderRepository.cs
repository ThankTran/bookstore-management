using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class OrderRepository : Repository<Order, string>, IOrderRepository
    {
        public OrderRepository(BookstoreDbContext context) : base(context) { }

        public async Task<IEnumerable<Order>> GetByCustomerAsync(string customerId)
        {
            return await DbSet
                .Where(o => o.CustomerId == customerId && o.DeletedDate == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByStaffAsync(string staffId)
        {
            return await DbSet
                .Where(o => o.StaffId == staffId && o.DeletedDate == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await DbSet
                .Where(o => o.CreatedDate >= start 
                            && o.CreatedDate <= end 
                            && o.DeletedDate == null)
                .ToListAsync();
        }
        
        
    }
}