using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;


namespace bookstore_Management.Data.Repositories.implementations
{
    internal class StaffRepository : Repository<Staff,string>, IStaffRepository
    {
        public StaffRepository(BookstoreDbContext context) : base(context) { }

        public IEnumerable<Staff> GetByStatus(StaffStatus status)
        {
            return Find(s => s.Status == status);
        }

        public IEnumerable<Staff> GetByRole(Role role)
        {
            return Find(s => s.Role == role);
        }

        public Staff GetByPhone(string phone)
        {
            return Find(s => s.Phone == phone).FirstOrDefault();
        }
    }
}