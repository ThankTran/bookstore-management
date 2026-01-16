using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin của hóa đơn nhập từ nhà cung cấp
    /// </summary>
    public class ImportBill
    {

        [Required]
        [StringLength(6)]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [StringLength(6)]
        [Column("publisher_id")]
        public string PublisherId { get; set; }
        
        [Required]
        [StringLength(6)]
        [Column("warehouse_id")]
        public string WarehouseId { get; set; }


        [Required]
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("notes")]
        [StringLength(500)]
        public string Notes { get; set; }

        [Required]
        [StringLength(6)]
        [Column("created_by")]
        public string CreatedBy { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public virtual ICollection<ImportBillDetail> ImportBillDetails { get; set; }
        public virtual Publisher Publisher { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}