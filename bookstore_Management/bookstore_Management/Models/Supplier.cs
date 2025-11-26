using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin của các nhà cung cấp cần quản lý
    /// </summary>
    internal class Supplier
    {
        // mã nhà cung cấp - khóa chính
        [Column("id")]
        [StringLength(6)]
        public string Id { get; set; }

        // tên nhà cung cấp
        [Required]
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } 
        
        // điện thoại nhà cung cấp
        [Required]
        [Column("phone")]
        [StringLength(30)]
        public string Phone { get; set; }
        
        // địa chỉ nhà cung cấp - có thể null
        [Column("address")]
        public string Address { get; set; }
        
        // email nhà cung cấp - có thể null
        [Column("email")]
        public string Email { get; set; }
        
        // navigation properties
        public virtual ICollection<Book> Books { get; set; }
    }
}
