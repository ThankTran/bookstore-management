using System;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.User.Response
{
    public class UserResponseDto
    {
        public string AccountId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string StaffId { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreateDate { get; set; }
    }
}