using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IStaffRepository : IRepository<Staff, string>
    {
        Task<IEnumerable<Staff>> GetByRoleAsync(UserRole userRole);
        IQueryable<Staff> SearchByName(string keyword);
    }
}