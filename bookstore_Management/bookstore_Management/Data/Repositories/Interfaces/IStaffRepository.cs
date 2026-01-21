using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    public interface IStaffRepository : IRepository<Staff,string>
    {
        IEnumerable<Staff> GetByRole(UserRole userRole);
        IEnumerable<Staff> SearchByName(string name);
    }
}