using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin cần quản lý của khách hàng
    /// Note :
    ///      + Hiển thị tổng chi của khách hàng ở service
    ///      + hạng thành viên chỉ tồn tại trong vòng 1 năm ( xem xét )
    ///      + Các khách hàng vãng lai không được lưu trữ
    /// </summary>
    public class Customer
    {
        // mã khách hàng - khóa chính
        [Column("id")]
        [StringLength(6)]
        public string CustomerId { get; set; }

        // tên khách hàng
        [Required]
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } 
        
        // điện thoại khách hàng
        [Required]
        [Column("phone")]
        [StringLength(30)]
        public string Phone { get; set; }
        
        // email - có thể null
        [Column("email")]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        
        // địa chỉ - có thể null
        [Column("address")]
        [StringLength(200)]
        public string Address { get; set; }
        
        // điểm tích lũy
        [Required]
        [Column("loyalty_points")]
        public decimal LoyaltyPoints { get; set; } = 0;

        // hạng thành viên
        [Required] 
        [Column("member_level")] 
        public MemberTier MemberLevel { get; set; } = MemberTier.WalkIn;
        
        // Navigation properties 
        public virtual ICollection<Order> Orders { get; set; } // 1 khách hàng có thể có nhiều hóa đơn

    }
}
