using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Ca làm việc được Admin phân công (theo ngày cụ thể)
    /// </summary>
    public class WorkSchedule
    {
        [Required]
        [StringLength(6)]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [StringLength(6)]
        [Column("week_id")]
        public string WeekId { get; set; }

        [Required]
        [StringLength(6)]
        [Column("staff_id")]
        public string StaffId { get; set; }

        [Required]
        [StringLength(6)]
        [Column("shift_template_id")]
        public string ShiftTemplateId { get; set; }

        [Required]
        [Column("work_date")]
        public DateTime WorkDate { get; set; } // Ngày cụ thể: 16/12/2024

        [Column("notes")]
        [StringLength(200)]
        public string Notes { get; set; }

        [Required]
        [StringLength(6)]
        [Column("assigned_by")]
        public string AssignedBy { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public virtual WorkWeek WorkWeek { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual ShiftTemplate ShiftTemplate { get; set; }
    }
}