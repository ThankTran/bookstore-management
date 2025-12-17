using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Tồn kho của từng sách tại từng kho
    /// </summary>
    public class Stock
    {
        [Required]
        [StringLength(6)]
        [Column("warehouse_id")]
        public string WarehouseId { get; set; }

        [Required]
        [StringLength(6)]
        [Column("book_id")]
        public string BookId { get; set; }

        [Required]
        [Column("quantity")]
        public int StockQuantity { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public virtual Book Book { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}