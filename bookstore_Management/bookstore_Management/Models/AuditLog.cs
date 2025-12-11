using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Ghi lại tất cả thay đổi trong hệ thống (để implement Undo/Redo)
    /// </summary>
    public class AuditLog
    {
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("entity_name")]
        public string EntityName { get; set; }  // "Book", "Stock", "Order", v.v.

        [Required]
        [StringLength(50)]
        [Column("entity_id")]
        public string EntityId { get; set; }  // ID của entity bị thay đổi (BookId, OrderId, v.v.)

        [Required]
        [StringLength(20)]
        [Column("action")]
        public string Action { get; set; }  // "Create", "Update", "Delete"

        [Column("old_values")]
        public string OldValues { get; set; }  // JSON của giá trị cũ (null nếu Create)

        [Column("new_values")]
        public string NewValues { get; set; }  // JSON của giá trị mới

        [Required]
        [StringLength(6)]
        [Column("changed_by")]
        public string ChangedBy { get; set; }  // StaffId - ai thực hiện thay đổi

        [Required]
        [Column("changed_date")]
        public DateTime ChangedDate { get; set; } = DateTime.Now;

        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }  // Mô tả chi tiết thay đổi
    }
}
