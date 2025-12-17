using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin cần quản lý của khách hàng
    /// Note:
    ///      + Hiển thị tổng chi của khách hàng ở service
    ///      + Hạng thành viên chỉ tồn tại trong vòng 1 năm (xem xét)
    ///      + Các khách hàng vãng lai không được lưu trữ
    /// </summary>
    public class Customer
    {
        [Required]
        [Column("id")]
        [StringLength(6)]
        public string CustomerId { get; set; }

        [Required]
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }
        
        [Required]
        [Column("phone")]
        [StringLength(20)]
        public string Phone { get; set; }
        
        [Required]
        [Column("member_level")]
        public MemberTier MemberLevel { get; set; } = MemberTier.Bronze;
        
        
        [Required]
        [Column("loyalty_points")]
        public decimal LoyaltyPoints { get; set; } = 0;

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        
        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; }
    }
}
