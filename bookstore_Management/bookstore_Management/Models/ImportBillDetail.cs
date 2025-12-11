using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Chi tiết của hóa đơn nhập
    /// </summary>
    public class ImportBillDetail
    {
        
        [Required]
        [StringLength(6)]
        [Column("import_id")]
        public int ImportId { get; set; }
        
        
        [Required]
        [StringLength(6)]
        [Column("book_id")]
        public string BookId { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("import_price")]
        public decimal ImportPrice { get; set; }

        [Required]
        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        // Navigation properties
        public virtual Book Book { get; set; }
        public virtual ImportBill Import { get; set; }
    }
}