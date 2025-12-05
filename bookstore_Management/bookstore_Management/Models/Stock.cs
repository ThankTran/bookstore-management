using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Tồn kho của từng sách
    /// </summary>
    public class Stock
    {
        [Required]
        [StringLength(6)]
        [Column("book_id")]
        public string BookId { get; set; }

        [Required]
        [Column("quantity")]
        public int StockQuantity { get; set; }

        // Navigation properties
        public virtual Book Book { get; set; }
    }
}