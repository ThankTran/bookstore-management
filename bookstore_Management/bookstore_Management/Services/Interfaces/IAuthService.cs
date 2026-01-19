using System.Threading.Tasks;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Auth;
using bookstore_Management.DTOs.User.Requests;

namespace bookstore_Management.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
    }
}