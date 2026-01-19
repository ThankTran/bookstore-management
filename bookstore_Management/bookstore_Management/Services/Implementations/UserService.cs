using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.User.Requests;
using bookstore_Management.DTOs.User.Response;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;
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

        public async Task<Result<string>> CreateUserAsync(CreateUserRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return Result<string>.Fail("Username và password bắt buộc");

            if (string.IsNullOrWhiteSpace(dto.StaffId))
                return Result<string>.Fail("Phải gắn với StaffId");

            // Parallel validation để giảm queries
            var staffTask = _staffRepository.GetByIdAsync(dto.StaffId);
            var usernameExistsTask = _userRepository.UsernameExistsAsync(dto.Username);

            await Task.WhenAll(staffTask, usernameExistsTask);

            var staff = staffTask.Result;
            if (staff == null || staff.DeletedDate != null)
                return Result<string>.Fail("Nhân viên không tồn tại");

            if (usernameExistsTask.Result)
                return Result<string>.Fail("Username đã tồn tại");

            var user = new User
            {
                Username = dto.Username.Trim(),
                PasswordHash = Encryptor.Hash(dto.Password),
                StaffId = dto.StaffId,
                UserRole = staff.UserRole,
                CreatedDate = DateTime.Now
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return Result<string>.Success(user.Username, "Tạo tài khoản thành công");
        }

        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
                return Result.Fail("Mật khẩu mới không được trống");

            if (dto.NewPassword.Length < 6)
                return Result.Fail("Mật khẩu phải có ít nhất 6 ký tự");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.DeletedDate != null)
                return Result.Fail("User không tồn tại");

            user.PasswordHash = Encryptor.Hash(dto.NewPassword);
            user.UpdatedDate = DateTime.Now;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Result.Success("Đổi mật khẩu thành công");
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

        public async Task<Result<UserResponseDto>> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || user.DeletedDate != null)
                return Result<UserResponseDto>.Fail("User không tồn tại");

            return Result<UserResponseDto>.Success(MapToDto(user));
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

        public async Task<Result<bool>> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
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

        private UserResponseDto MapToDto(User user)
        {
            return new UserResponseDto
            {
                UserName = user.Username,
                Password = user.PasswordHash,
                StaffId = user.StaffId,
                CreateDate = user.CreatedDate
            };
        }
    }
}