using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin kho hàng
    /// </summary>
    public class Warehouse
    {
        [Required]
        [StringLength(6)]
        [Column("id")]
        public string WarehouseId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [StringLength(200)]
        [Column("address")]
        public string Address { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public virtual ICollection<Stock> Stocks { get; set; }
        public virtual ICollection<ImportBill> ImportBills { get; set; }
    }

}