using System.Threading.Tasks;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IUserRepository : IRepository<User, string>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
    }
}