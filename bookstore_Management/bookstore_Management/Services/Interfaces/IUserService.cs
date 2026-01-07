using System;
using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.User.Requests;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Interfaces
{
    internal interface IUserService
    {
        Result<string> CreateUser(CreateUserRequestDto dto);
        Result ChangePassword(string userId, ChangePasswordRequestDto dto);
        Result Deactivate(string userId);
        Result<User> GetById(string userId);
        Result<User> GetByUsername(string username);
        Result<UserRole> GetUserRole(string userId);
        Result<bool> Login(string username, string password);
        Result<IEnumerable<User>> GetAll();
    }
}
