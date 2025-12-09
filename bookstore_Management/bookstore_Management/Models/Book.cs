using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin cần lưu trữ của sách
    /// </summary>
    public class Book
    {
        [Required]
        [StringLength(6)]
        [Column("id")]
        public string BookId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [StringLength(6)]
        [Column("supplier_id")]
        public string SupplierId { get; set; }

        [Required]
        [Column("category")]
        public BookCategory Category { get; set; }

        [Column("sale_price", TypeName = "decimal(18,2)")]
        public decimal? SalePrice { get; set; }

        [Required]
        [Column("import_price", TypeName = "decimal(18,2)")]
        public decimal ImportPrice { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
