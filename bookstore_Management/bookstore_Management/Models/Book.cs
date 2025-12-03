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
    /// Các thông tin cần lưu trữ của sách
    /// </summary>
    public class Book
    {
        // mã sách - khóa chính 
        [Required]
        [StringLength(6)]
        [Column("id")]
        public string BookId { get; set; }

        // tên sách
        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }

        // mã nhà cung cấp - khóa ngoại
        [Required]
        [StringLength(6)]
        [Column("Supplier_id")]
        public string SupplierId { get; set; }
        
        // thể loại sách 
        [Required]
        [Column("category")]
        public BookCategory Category { get; set; }

        // giá bán - có thể null
        [Column("sale_price")]
        [DataType(DataType.Currency)]
        public decimal? SalePrice { get; set; }  

        // giá nhập
        [Required]
        [Column("import_price")]
        [DataType(DataType.Currency)]
        public decimal ImportPrice { get; set; }
        
        // navigation properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Supplier Supplier { get; set; }
        
    }
}
