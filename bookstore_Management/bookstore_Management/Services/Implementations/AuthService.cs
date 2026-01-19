using System;
using System.Linq;
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


        public Result<LoginResponseDto> Login(LoginRequestDto dto)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                    return Result<LoginResponseDto>.Fail("Tên đăng nhập hoặc mật khẩu không được để trống");

                // Kiểm tra Username
                var user = _userRepository.GetByUsername(dto.Username.Trim()).FirstOrDefault();
                
                // nếu username không tồn tại thì trả về false
                if (user == null)
                    return Result<LoginResponseDto>.Fail("Tên đăng nhập hoặc mật khẩu không đúng");

                // kiểm tra mật khẩu
                if (!Encryptor.Verify(dto.Password, user.PasswordHash))
                    return Result<LoginResponseDto>.Fail("Tên đăng nhập hoặc mật khẩu không đúng");

                // trả về kết quả
                var response = new LoginResponseDto
                {
                    Username = user.Username,
                    Role = user.UserRole,
                    RoleName = user.UserRole.GetDisplayName(),
                    IsActive = user.DeletedDate == null
                };

                return Result<LoginResponseDto>.Success(response, "Đăng nhập thành công");
            }
            catch (Exception ex)
            {
                return Result<LoginResponseDto>.Fail($"Lỗi hệ thống: {ex.Message}");
            }
        }

     
    }
}
