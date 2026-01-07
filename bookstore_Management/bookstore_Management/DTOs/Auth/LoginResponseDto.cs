using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Auth
{
    /// <summary>
    /// DTO for login response
    /// Contains user information after successful login
    /// Must NOT contain password, password hash, or salt
    /// </summary>
    public class LoginResponseDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public UserRole Role { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
}

