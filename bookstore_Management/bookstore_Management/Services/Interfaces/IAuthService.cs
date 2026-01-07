using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Auth;
using bookstore_Management.DTOs.User.Requests;

namespace bookstore_Management.Services.Interfaces
{
    internal interface IAuthService
    {
        Result<LoginResponseDto> Login(LoginRequestDto dto);
        Result ChangePassword(string userId, ChangePasswordRequestDto dto);
    }
}
