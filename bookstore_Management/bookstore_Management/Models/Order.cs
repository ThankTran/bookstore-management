using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin cần quản lý của hóa đơn - tổng quát
    /// </summary>
    internal class Order
    {
        // mã hóa đơn - khóa chính
        [Required]
        [StringLength(6)]
        [Column("order_id")]
        public string OrderId { get; set; }

        // mã nhân viên - khóa ngoại
        [Required]
        [StringLength(6)]
        [Column("staff_id")]
        public string StaffId { get; set; }

        // CustomerId có thể NULL (khách vãng lai) - khóa ngoại
        [StringLength(6)]
        [Column("customer_id")]
        public string CustomerId { get; set; }

        // phương thức thanh toán
        [Required]
        [StringLength(50)]
        [Column("payment_method")]
        public PaymentType PaymentMethod { get; set; }

        // giảm giá
        [Required]
        [Column("discount")] 
        public decimal? Discount { get; set; } = 0;

        // tổng tiền
        [Required]
        [Column("total_price")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }
        
        // ghi chú
        [Column("note")]
        [StringLength(500)]
        public string Notes { get; set; }
        
        // Navigation properties 
        public virtual Staff Staff { get; set; } // chỉ có 1 nhân viên tọa hóa đơn
        public virtual Customer Customer { get; set; } // chỉ có 1 hoặc không có khách hàng
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } // có 1 hoặc nhiều chi tiết đơn hàng

    }
}
