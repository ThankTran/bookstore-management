using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Interfaces
{
    internal interface IAuthService
    {
        Result<User> Login(LoginDto dto);
        Result ChangePassword(string userId, ChangePasswordDto dto);
    }
}
