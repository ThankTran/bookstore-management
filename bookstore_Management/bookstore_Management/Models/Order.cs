using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin của hóa đơn bán hàng
    /// </summary>
    public class Order
    {
        [Required]
        [StringLength(6)]
        [Column("id")]
        public string OrderId { get; set; }

        [Required]
        [StringLength(6)]
        [Column("staff_id")]
        public string StaffId { get; set; }

        [StringLength(6)]
        [Column("customer_id")]
        public string CustomerId { get; set; }

        [Required]
        [Column("payment_method")]
        public PaymentType PaymentMethod { get; set; }

        [Required]
        [Column("discount", TypeName = "decimal(18,2)")]
        public decimal? Discount { get; set; } = 0;

        [Required]
        [Column("total_price", TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Column("note")]
        [StringLength(500)]
        public string Notes { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public virtual Staff Staff { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
