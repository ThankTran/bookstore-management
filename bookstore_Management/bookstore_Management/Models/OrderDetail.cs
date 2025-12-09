

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Chi tiết của hóa đơn bán hàng
    /// </summary>
    public class OrderDetail
    {
        [Required]
        [StringLength(6)]
        [Column("order_id")]
        public string OrderId { get; set; }

        [Required]
        [StringLength(6)]
        [Column("book_id")]
        public string BookId { get; set; }

        [Required]
        [Column("sale_price", TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("subtotal", TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column("note")]
        [StringLength(500)]
        public string Notes { get; set; }

        // Navigation properties
        public virtual Book Book { get; set; }
        public virtual Order Order { get; set; }
    }
}
