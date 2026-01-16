using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IUserRepository : IRepository<User, string>
    {
        User GetByUsername(string username);
        bool UsernameExists(string username);
    }
}
