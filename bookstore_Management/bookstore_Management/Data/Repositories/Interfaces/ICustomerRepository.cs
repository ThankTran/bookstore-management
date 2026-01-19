
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface ICustomerRepository : IRepository<Customer,string>
    {
        IQueryable<Customer> SearchByName(string keyword);
        Task<IEnumerable<Customer>>GetByMemberLevelAsync(MemberTier tier);
        Task<Customer> SearchByPhoneAsync(string phone);
        Task<bool> UsePointAsync(string customerId, decimal points);
        Task<bool> AddPointAsync(string customerId, decimal points);
        
        /// <summary>
        /// Gets all active (non-deleted) customers for list view
        /// Filters by DeletedDate == null
        /// </summary>
        Task<IEnumerable<Customer>> GetAllForListViewAsync();
    }
}
