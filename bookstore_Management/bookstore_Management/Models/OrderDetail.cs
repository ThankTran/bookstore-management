

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin cần quản lí của hóa đơn - chi tiết
    /// </summary>
    public class OrderDetail
    {
        // mã hóa đơn - khóa chính - khóa ngoại
        [Required]
        [StringLength(20)]
        [Column("Order_id")]
        public string OrderId { get; set; }

        // mã sách - khóa chính - khóa ngoại
        [Required]
        [StringLength(6)] 
        [Column("book_id")] 
        public string BookId { get; set; } 
        
        // tiền bán tại thời điểm đó
        [Required]
        [Column("sale_price")]
        [DataType(DataType.Currency)]
        public decimal SalePrice { get; set; }  

        // số lượng
        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }
        
        // ghi chú
        [Column("note")]
        [StringLength(500)]
        public string Notes { get; set; }
        
        // navigation properties
        public virtual Book Book { get; set; } 
        public virtual Order Order { get; set; } 
    
    }
}
