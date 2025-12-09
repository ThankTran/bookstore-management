using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IStaffRepository : IRepository<Staff,string>
    {
        IEnumerable<Staff> GetByStatus(StaffStatus status);
        IEnumerable<Staff> GetByRole(Role role);
        Staff GetByPhone(string phone);
    }
}