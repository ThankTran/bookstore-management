using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Doanh thu hàng ngày của nhân viên
    /// ReadOnly
    /// </summary>
    public class StaffDailyRevenue
    {
        [Required]
        [StringLength(6)]
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Required]
        [Column("day")]
        public DateTime Day { get; set; } = DateTime.Now;

        [Required]
        [Column("revenue", TypeName = "decimal(18,2)")]
        public decimal Revenue { get; set; } = 0;

        // Navigation properties
        public virtual Staff Staff { get; set; }
    }
}