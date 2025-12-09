using System.ComponentModel.DataAnnotations;

namespace bookstore_Management.DTOs
{
    /// <summary>
    /// DTO cho đăng nhập
    /// </summary>
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// DTO cho đổi mật khẩu
    /// </summary>
    public class ChangePasswordDto
    {

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// DTO cho tạo user mới
    /// </summary>
    public class CreateUserDto
    {
        
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } = "Staff";
    }
}