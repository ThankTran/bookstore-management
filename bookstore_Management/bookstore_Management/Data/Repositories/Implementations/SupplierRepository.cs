using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class SupplierRepository : Repository<Supplier, string>, ISupplierRepository
    {
        public SupplierRepository(BookstoreDbContext context) : base(context)
        {
        }

        public IEnumerable<Supplier> SearchByName(string name)
        {
            return Find(s => s.Name.Contains(name));
        }

        public Supplier GetByPhone(string phone)
        {
            return Find(s => s.Phone == phone).FirstOrDefault();
        }

        public Supplier SearchByEmail(string email)
        {
            return Find(s => s.Email == email).FirstOrDefault();
        }
    }
}
