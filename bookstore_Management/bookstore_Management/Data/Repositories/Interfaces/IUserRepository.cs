using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IUserRepository : IRepository<User, string>
    {
       IEnumerable<User> GetByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
    }
}