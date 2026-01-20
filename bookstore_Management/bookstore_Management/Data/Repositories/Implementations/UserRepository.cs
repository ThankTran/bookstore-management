using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class UserRepository : Repository<User, string>, IUserRepository
    {
        public UserRepository(BookstoreDbContext context) : base(context) { }

        /// <summary>
        /// Gets user by username, đã lọc soft delete (async)
        /// </summary>
        public IEnumerable<User> GetByUsernameAsync(string username)
        {
            return DbSet
                .Where(u => u.Username == username && u.DeletedDate == null);
        }

        /// <summary>
        /// Kiểm tra Username có tồn tại không, đã lọc soft delete (async)
        /// </summary>
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await DbSet
                .AnyAsync(u => u.Username == username && u.DeletedDate == null);
        }
    }
}