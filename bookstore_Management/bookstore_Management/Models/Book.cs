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
        [Column("category")]
        public BookCategory Category { get; set; }

        [Column("sale_price")]
        public decimal? SalePrice { get; set; }

        [Required]
        [Column("import_price")]
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
        public virtual Stock Stock { get; set; }
        
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }
    }
}
