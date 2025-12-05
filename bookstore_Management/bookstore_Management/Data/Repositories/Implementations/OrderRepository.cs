using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bookstore_Management.Data.Context;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories
{
    internal class OrderRepository : Repository<Order,string>, IOrderRepository
    {
        public OrderRepository(BookstoreDbContext context) : base(context) { }

        public IEnumerable<Order> GetByCustomer(string customerId)
        {
            return Find(o => o.CustomerId == customerId);
        }

        public IEnumerable<Order> GetByStaff(string staffId)
        {
            return Find(o => o.StaffId == staffId);
        }

        public IEnumerable<Order> GetByDateRange(DateTime start, DateTime end)
        {
            return Find(o => o.CreatedDate >= start && o.CreatedDate <= end);
        }
    }
}
