using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Định nghĩa các ca làm việc cố định
    /// </summary>
    public class ShiftTemplate
    {
        [Required]
        [StringLength(6)]
        [Column("id")]
        public string Id { get; set; } // ID0001, ID0002

        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; } // "Ca chiều", "Ca sáng"

        [Required]
        [Column("start_time")]
        public TimeSpan StartTime { get; set; } // 12:30, 08:00

        [Required]
        [Column("end_time")]
        public TimeSpan EndTime { get; set; } // 17:30, 13:00

        [Required]
        [Column("working_days")]
        [StringLength(50)]
        public string WorkingDays { get; set; } // "1,2,3,4,5" (T2-T6) hoặc JSON
        
        [Column("description")]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public virtual ICollection<StaffShiftRegistration> StaffShiftRegistrations { get; set; }
        public virtual ICollection<WorkSchedule> WorkSchedules { get; set; }
    }
}