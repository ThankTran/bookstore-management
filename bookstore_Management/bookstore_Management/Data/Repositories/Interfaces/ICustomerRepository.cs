
using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface ICustomerRepository : IRepository<Customer,string>
    {
        IEnumerable<Customer> SearchByName(string name);
        IEnumerable<Customer> GetByMemberLevel(MemberTier tier);
        Customer SearchByPhone(string phone);
        
        /// <summary>
        /// Gets all active (non-deleted) customers for list view
        /// Filters by DeletedDate == null
        /// </summary>
        IEnumerable<Customer> GetAllForListView();
    }
}
