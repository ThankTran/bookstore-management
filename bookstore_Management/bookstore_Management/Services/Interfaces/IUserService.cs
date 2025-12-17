using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Interfaces
{
    internal interface IUserService
    {
        Result<string> CreateUser(CreateUserDto dto);
        Result ChangePassword(string userId, ChangePasswordDto dto);
        Result Deactivate(string userId);
        Result<User> GetById(string userId);
        Result<User> GetByUsername(string username);
        Result<IEnumerable<User>> GetAll();
    }
}
