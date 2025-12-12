using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

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
        public string ImportId { get; set; }
        
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

        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public virtual Book Book { get; set; }
        public virtual ImportBill Import { get; set; }
    }
}