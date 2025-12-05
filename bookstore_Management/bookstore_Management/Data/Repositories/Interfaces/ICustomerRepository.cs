using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories
{
    internal interface ICustomerRepository : IRepository<Customer,string>
    {
        IEnumerable<Customer> SearchByName(string name);
        IEnumerable<Customer> SearchByEmail(string email);
        IEnumerable<Customer> GetByMemberLevel(MemberTier tier);
        Customer SearchByPhone(string phone);
    }
}
