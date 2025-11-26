using System.ComponentModel.DataAnnotations;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Models
{
    
    /// <summary>
    /// Thông tin cần quản lý của tài khoản
    /// </summary>
    internal class User
    {
        [Key]
        [StringLength(6)]
        public string UserId { get; set; }  // mã số nhân viên : NV001,NV002,...

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }
        
    }
}    
