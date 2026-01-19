using System;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Auth;
using bookstore_Management.DTOs.User.Requests;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Utils;

namespace bookstore_Management.Services.Implementations
{
    /// <summary>
    /// Authentication service implementing secure login flow
    /// Uses existing Encryptor utility for password hashing/verification
    /// Follows strict login sequence and does not reveal specific error details
    /// </summary>
    internal class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        internal AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
        {
            // ✅ Input validation
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return Result<LoginResponseDto>.Fail("Tên đăng nhập hoặc mật khẩu không được để trống");

            // ✅ Single query - no N+1
            var user = await _userRepository.GetByUsernameAsync(dto.Username.Trim());
            
            // ✅ Security: Generic error message (prevents username enumeration)
            if (user == null)
                return Result<LoginResponseDto>.Fail("Tên đăng nhập hoặc mật khẩu không đúng");

            // ✅ Security: Constant-time password verification
            if (!Encryptor.Verify(dto.Password, user.PasswordHash))
                return Result<LoginResponseDto>.Fail("Tên đăng nhập hoặc mật khẩu không đúng");

            // ✅ Return minimal user info
            var response = new LoginResponseDto
            {
                Username = user.Username,
                Role = user.UserRole,
                RoleName = user.UserRole.GetDisplayName(),
                IsActive = user.DeletedDate == null
            };

            return Result<LoginResponseDto>.Success(response, "Đăng nhập thành công");
        }
    }
}

