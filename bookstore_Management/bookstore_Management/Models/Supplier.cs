using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin của nhà cung cấp
    /// </summary>
    public class Supplier
    {
        [Required]
        [StringLength(6)]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        [Column("phone")]
        public string Phone { get; set; }

        [Column("address")]
        [StringLength(200)]
        public string Address { get; set; }

        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public virtual ICollection<Book> Books { get; set; }
        public virtual ICollection<ImportBill> ImportBills { get; set; }
    }
}
