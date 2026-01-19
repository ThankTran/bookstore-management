using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class OrderDetailRepository 
        : Repository<OrderDetail, (string BookId, string OrderId)>, IOrderDetailRepository
    {
        public OrderDetailRepository(BookstoreDbContext context) : base(context) { }

        public async Task<IEnumerable<OrderDetail>> GetByOrderAsync(string orderId)
        {
            return await DbSet
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetByBookAsync(string bookId)
        {
            return await DbSet
                .Where(od => od.BookId == bookId )
                .ToListAsync();
        }
    }
}