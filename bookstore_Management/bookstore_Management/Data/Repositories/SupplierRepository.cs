using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bookstore_Management.Data.Context;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories
{
    internal class SupplierRepository : Repository<Supplier,string>, ISupplierRepository
    {
        public SupplierRepository(BookstoreDbContext context) : base(context) { }

        public IEnumerable<Supplier> SearchByName(string name)
        {
            return Find(s => s.Name.Contains(name));
        }
    }
}
