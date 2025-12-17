using System;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Utils;

namespace bookstore_Management.Services.Implementations
{
    internal class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        internal AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Result<User> Login(LoginDto dto)
        {
            try
            {
                // Chỉ validate đầu vào và verify hash, không ném exception để UI dễ xử lý
                if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                    return Result<User>.Fail("Thiếu thông tin đăng nhập");

                var user = _userRepository.GetByUsername(dto.Username.Trim());
                if (user == null || user.DeletedDate != null)
                    return Result<User>.Fail("Tài khoản không tồn tại hoặc đã bị khóa");

                if (!Encryptor.Verify(dto.Password, user.PasswordHash))
                    return Result<User>.Fail("Sai mật khẩu");

                return Result<User>.Success(user, "Đăng nhập thành công");
            }
            catch (Exception ex)
            {
                return Result<User>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result ChangePassword(string userId, ChangePasswordDto dto)
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
