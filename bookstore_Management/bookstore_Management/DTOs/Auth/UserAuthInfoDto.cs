using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Auth
{
    /// <summary>
    /// Internal DTO for authentication information
    /// Used within service layer only, not exposed to UI
    /// </summary>
    internal class UserAuthInfoDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }
}

