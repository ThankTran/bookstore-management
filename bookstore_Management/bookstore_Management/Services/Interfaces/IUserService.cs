using System.Collections.Generic;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.User.Requests;
using bookstore_Management.DTOs.User.Response;

namespace bookstore_Management.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<string>> CreateUserAsync(CreateUserRequestDto dto);

        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequestDto dto);

        Task<Result> DeactivateAsync(string userId);

        Task<Result<UserResponseDto>> GetByIdAsync(string userId);

        Task<Result<UserResponseDto>> GetByUsernameAsync(string username);

        Task<Result<IEnumerable<UserResponseDto>>> GetAllAsync();

        Task<Result<bool>> LoginAsync(string username, string password);

        Task<Result<UserRole>> GetUserRoleAsync(string userId);
    }
}