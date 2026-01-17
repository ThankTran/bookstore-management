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

        public Result Deactivate(string userId)
        {
            try
            {
                var user = _userRepository.GetById(userId);
                if (user == null || user.DeletedDate != null)
                    return Result.Fail("User không tồn tại");

                user.DeletedDate = DateTime.Now;
                _userRepository.Update(user);
                _userRepository.SaveChanges();
                return Result.Success("Đã khóa tài khoản");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<UserResponseDto> GetById(string userId)
        {
            try
            {
                var user = _userRepository.GetById(userId);
                if (user == null || user.DeletedDate != null)
                    return Result<UserResponseDto>.Fail("User không tồn tại");


                var dto = MapToStaffResponseDto(user);
                
                return Result<UserResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<UserResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<UserResponseDto> GetByUsername(string username)
        {
            try
            {
                var user = _userRepository.GetByUsername(username);
                if (user == null || user.DeletedDate != null)
                    return Result<UserResponseDto>.Fail("User không tồn tại");

                var dto = MapToStaffResponseDto(user);
                return Result<UserResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<UserResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<UserResponseDto>> GetAll()
        {
            try
            {
                var users = _userRepository.GetAll().Where(u => u.DeletedDate == null)
                    .Select( MapToStaffResponseDto);
                return Result<IEnumerable<UserResponseDto>>.Success(users);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<UserResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<bool> Login(string username, string password)
        {
            try
            {
                var  user = _userRepository.GetByUsername(username);
                if (user == null || user.DeletedDate != null)
                    return Result<bool>.Fail("User không tồn tại");
                return  (!Encryptor.Verify(password, user.PasswordHash)) ? 
                    Result<bool>.Success(true) : 
                    Result<bool>.Success(false);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<UserRole> GetUserRole(string userId)
        {
            try
            {
                var role = _userRepository.GetById(userId).UserRole;
                return Result<UserRole>.Success(role);
            }
            catch (Exception ex)
            {
                return Result<UserRole>.Fail($"Lỗi: {ex.Message}");
            }
        }
        
        private UserResponseDto MapToStaffResponseDto(User user)
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

