using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin của nhân viên
    /// </summary>
    public class Staff
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
        [Column("base_salary", TypeName = "decimal(18,2)")]
        public decimal BaseSalary { get; set; }

        [Required]
        [StringLength(12)]
        [Column("citizen_id_card")]
        public string CitizenIdCard { get; set; }

        [Required]
        [StringLength(20)]
        [Column("phone")]
        public string Phone { get; set; }

        [Required]
        [Column("status")]
        public StaffStatus Status { get; set; }

        [Required]
        [Column("role")]
        public Role Role { get; set; } = Role.SalesStaff;

        [Required]
        [Column("salary_rate", TypeName = "decimal(18,2)")]
        public decimal SalaryRate { get; set; } = 0;

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("deleted_date")]
        public DateTime? DeletedDate { get; set; }

        // Navigation properties
        public virtual ICollection<StaffDailyRevenue> DailyRevenues { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}