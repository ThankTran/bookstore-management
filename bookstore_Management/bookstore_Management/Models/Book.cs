using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin cần lưu trữ của sách
    /// </summary>
    public class Book
    {
        // mã sách
        [Required]
        [StringLength(6)]
        [Column("id")]
        public string BookId { get; set; }

        // tên sách
        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }
        
        // tác giả
        [Required]
        [StringLength(50)]
        [Column("author")]
        public string Author { get; set; }
        
        // dùng để truy vấn nhà xuất bản
        [StringLength(6)]
        [Column("publisher_id")]
        public string PublisherId { get; set; }

        // thể loại
        [Required]
        [Column("category")]
        public BookCategory Category { get; set; }

        // giá bán
        [Column("sale_price")]
        public decimal? SalePrice { get; set; }
        
        [Column("stock")]
        public int Stock { get; set; }

        // các thông tin để kiểm tra
        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Publisher Publisher { get; set; }
        
        
    }
}
