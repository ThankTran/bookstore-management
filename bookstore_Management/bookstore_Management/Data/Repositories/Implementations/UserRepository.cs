using System.Collections.Generic;
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

        /// <summary>
        /// Gets user by username, đã lọc soft delete
        /// </summary>
        public User GetByUsername(string username)
        {
            return Find(u => u.Username == username && u.DeletedDate == null).FirstOrDefault();
        }

        /// <summary>
        /// Kiểm tra Username có tồn tại không, đã lọc soft delete
        /// </summary>
        public bool UsernameExists(string username)
        {
            return Exists(u => u.Username == username && u.DeletedDate == null);
        }
    }
}

