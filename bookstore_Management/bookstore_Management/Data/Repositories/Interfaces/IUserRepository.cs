using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IUserRepository : IRepository<bookstore_Management.Models.User, string>
    {
        bookstore_Management.Models.User GetByUsername(string username);
        bool UsernameExists(string username);
    }
}
