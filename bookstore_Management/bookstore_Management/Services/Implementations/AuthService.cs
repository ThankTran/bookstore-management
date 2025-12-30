using System;
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

        /// <summary>
        /// Login flow following strict sequence:
        /// 1. Validate input (username/password not empty)
        /// 2. Query user by username (excludes soft-deleted users)
        /// 3. If user not found → login fails
        /// 4. If user is inactive (soft-deleted) → deny login
        /// 5. Use existing encryption utility to verify password
        /// 6. Compare hashed input with stored hash
        /// 7. If mismatch → login fails
        /// 8. If valid → return LoginResponseDto (NO password data)
        /// 
        /// Security: Does NOT reveal whether username or password is incorrect
        /// </summary>
        public Result<LoginResponseDto> Login(LoginRequestDto dto)
        {
            try
            {
                // Step 1: Validate input
                if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                    return Result<LoginResponseDto>.Fail("Tên đăng nhập hoặc mật khẩu không được để trống");

                // Step 2: Query user by username (repository already filters soft-deleted users)
                var user = _userRepository.GetByUsername(dto.Username.Trim());
                
                // Step 3 & 4: If user not found or inactive → login fails
                // Use generic message to not reveal if username exists
                if (user == null)
                    return Result<LoginResponseDto>.Fail("Tên đăng nhập hoặc mật khẩu không đúng");

                // Step 5 & 6: Use existing encryption utility to verify password
                if (!Encryptor.Verify(dto.Password, user.PasswordHash))
                    return Result<LoginResponseDto>.Fail("Tên đăng nhập hoặc mật khẩu không đúng");

                // Step 7 & 8: If valid → return LoginResponseDto (NO password data exposed)
                var response = new LoginResponseDto
                {
                    UserId = user.UserId,
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

        public Result ChangePassword(string userId, ChangePasswordRequestDto dto)
        {
            try
            {
                // Không dùng exception; trả Result để caller hiển thị thông báo rõ ràng
                var user = _userRepository.GetById(userId);
                if (user == null || user.DeletedDate != null)
                    return Result.Fail("Tài khoản không tồn tại hoặc đã bị khóa");

                if (!Encryptor.Verify(dto.OldPassword, user.PasswordHash))
                    return Result.Fail("Mật khẩu cũ không đúng");

                if (dto.NewPassword != dto.ConfirmPassword)
                    return Result.Fail("Mật khẩu xác nhận không khớp");

                user.PasswordHash = Encryptor.Hash(dto.NewPassword);
                user.UpdatedDate = DateTime.Now;
                _userRepository.Update(user);
                _userRepository.SaveChanges();
                return Result.Success("Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }
    }
}
