using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;


namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class StaffRepository : Repository<Staff,string>, IStaffRepository
    {
        public StaffRepository(BookstoreDbContext context) : base(context) { }
        

        public IEnumerable<Staff> GetByRole(UserRole userRole)
        {
            return Find(s => s.UserRole == userRole);
        }

        public IEnumerable<Staff> SearchByName(string name)
        {
            return Find(s => s.Name.Contains(name) && s.DeletedDate == null);
        }
    }
}