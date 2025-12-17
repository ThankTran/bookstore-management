using System.Linq;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class UserRepository : Repository<User, string>, IUserRepository
    {
        public UserRepository(BookstoreDbContext context) : base(context) { }

        public User GetByUsername(string username)
        {
            return Find(u => u.Username == username).FirstOrDefault();
        }

        public bool UsernameExists(string username)
        {
            return Exists(u => u.Username == username);
        }
    }
}

