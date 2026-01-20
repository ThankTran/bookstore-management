using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.User.Requests;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Core.Utils;
using bookstore_Management.DTOs.User.Response;
using bookstore_Management.Utils;

namespace bookstore_Management.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStaffRepository _staffRepository;

        internal UserService(IUserRepository userRepository, IStaffRepository staffRepository)
        {
            _userRepository = userRepository;
            _staffRepository = staffRepository;
        }

        public Result<string> CreateUser(CreateUserRequestDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                    return Result<string>.Fail("Username và password bắt buộc");

                if (string.IsNullOrWhiteSpace(dto.StaffId))
                    return Result<string>.Fail("Phải gắn với StaffId");

                var staff = _staffRepository.GetById(dto.StaffId);
                if (staff == null || staff.DeletedDate != null)
                    return Result<string>.Fail("Nhân viên không tồn tại");

                if (_userRepository.UsernameExists(dto.Username))
                    return Result<string>.Fail("Username đã tồn tại");

                var user = new User
                {
                    Username = dto.Username.Trim(),
                    PasswordHash = Encryptor.Hash(dto.Password),
                    StaffId = dto.StaffId,
                    CreatedDate = DateTime.Now
                };

                _userRepository.Add(user);
                _userRepository.SaveChanges();
                return Result<string>.Success(user.Username, "Tạo tài khoản thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<UserResponseDto> GetByUsername(string username)
        {
            var user = _userRepository.GetByUsername(username)
                .FirstOrDefault(u => u.DeletedDate == null);

            if (user == null)
                return Result<UserResponseDto>.Fail("User không tồn tại");

            return Result<UserResponseDto>.Success(MapToUserResponseDto(user));
        }

        public Result ChangePassword(string userId, ChangePasswordRequestDto dto)
        {
            try
            {
                var user = _userRepository.GetById(userId);
                if (user == null || user.DeletedDate != null)
                    return Result.Fail("User không tồn tại");

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

        public async Task<Result> DeactivateAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.DeletedDate != null)
                return Result.Fail("User không tồn tại");

            user.DeletedDate = DateTime.Now;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Result.Success("Đã khóa tài khoản");
        }

        public async Task<Result<UserResponseDto>> GetByIdAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.DeletedDate != null)
                return Result<UserResponseDto>.Fail("User không tồn tại");

            return Result<UserResponseDto>.Success(MapToDto(user));
        }

        public Result<IEnumerable<UserResponseDto>> GetByUsername(string username)
        {
            var user = _userRepository.GetByUsernameAsync(username).Select(MapToDto);

            return Result<IEnumerable<UserResponseDto>>.Success(user);
        }

        public async Task<Result<IEnumerable<UserResponseDto>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var filtered = users
                .Where(u => u.DeletedDate == null)
                .Select(MapToDto)
                .ToList();

            return Result<IEnumerable<UserResponseDto>>.Success(filtered);
        }

        public Result<bool> LoginAsync(string username, string password)
        {
            var user = _userRepository.GetByUsernameAsync(username).FirstOrDefault();
            if (user == null || user.DeletedDate != null)
                return Result<bool>.Fail("User không tồn tại");

            bool match = Encryptor.Verify(password, user.PasswordHash);

            return Result<bool>.Success(match);
        }

        public async Task<Result<UserRole>> GetUserRoleAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.DeletedDate != null)
                return Result<UserRole>.Fail("User không tồn tại");

            return Result<UserRole>.Success(user.UserRole);
        }

        private UserResponseDto MapToUserResponseDto(User user)
        {
            return new UserResponseDto
            {
                UserName = user.Username.Trim(),
                Password = user.PasswordHash,
                StaffId = user.StaffId,
                CreateDate = user.CreatedDate
            };
        }

    }
}