using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Models
{

    /// <summary>
    /// Thông tin cần quản lý của tài khoản
    /// </summary>
    public class User
    {
        [Key]
        [Column("user_id")]
        [StringLength(10)] // tăng lên để tránh tràn
        public string UserId { get; set; } // mã số nhân viên : NV001,NV002,...

        [Required] 
        [Column("username")]
        [StringLength(50)] 
        public string Username { get; set; }

        [Required] 
        [Column("password_hash")]
        [StringLength(255)] 
        public string PasswordHash { get; set; }

        
        [StringLength(100)] 
        [Column("staff_id")]
        public string StaffId { get; set; }

        [Required] 
        [Column("role")]
        public UserRole UserRole { get; set; } // enum: Admin, Staff, Customer

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

    }
}    
