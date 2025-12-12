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
    internal class CustomerRepository : Repository<Customer,string>,ICustomerRepository
    {
        public CustomerRepository(BookstoreDbContext context) : base(context) { }

        public IEnumerable<Customer> SearchByName(string name)
        {
            return Find(c => c.Name.Contains(name));
        }
        

        public IEnumerable<Customer> GetByMemberLevel(MemberTier tier)
        {
            return Find(c => c.MemberLevel == tier);
        }
        
        public Customer SearchByPhone(string phone)
        {
            return Find(c => c.Phone == phone).FirstOrDefault();
        }
        
    }
}
