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

        public Result<User> GetById(string userId)
        {
            try
            {
                var user = _userRepository.GetById(userId);
                if (user == null || user.DeletedDate != null)
                    return Result<User>.Fail("User không tồn tại");
                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<User> GetByUsername(string username)
        {
            try
            {
                var user = _userRepository.GetByUsername(username);
                if (user == null || user.DeletedDate != null)
                    return Result<User>.Fail("User không tồn tại");
                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<User>> GetAll()
        {
            try
            {
                var users = _userRepository.GetAll().Where(u => u.DeletedDate == null).ToList();
                return Result<IEnumerable<User>>.Success(users);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<User>>.Fail($"Lỗi: {ex.Message}");
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
        
    }
}

