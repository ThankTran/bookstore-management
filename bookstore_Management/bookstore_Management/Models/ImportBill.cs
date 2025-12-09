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
        [Column("id")]
        public int ImportBillId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("code")]
        public string ImportBillCode { get; set; }

        [Required]
        [Column("import_date")]
        public DateTime ImportDate { get; set; }

        [Required]
        [StringLength(6)]
        [Column("supplier_id")]
        public string SupplierId { get; set; }

        [Required]
        [Column("total_amount", TypeName = "decimal(18,2)")]
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

        // Navigation properties
        public virtual ICollection<ImportBillDetail> ImportBillDetails { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}