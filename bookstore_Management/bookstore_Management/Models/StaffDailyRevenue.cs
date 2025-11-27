using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin cần quản lí của nhân viên - doanh thu trên tháng
    /// </summary>
    public class StaffDailyRevenue
    {
        // mã nhân viên - khóa chính - khóa ngoại
        [Required]
        [StringLength(6)]
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        // ngày làm - khóa chính 
        [Required]
        [Column("day")]
        [DataType(DataType.Date)]
        public DateTime Day { get; set; } = DateTime.Now;

        // doanh thu
        [Required]
        [Column("revenue")]
        [DataType(DataType.Currency)]
        public decimal Revenue { get; set; } = 0;
        
        // navigation properties
        public virtual Staff Staff { get; set; } // 1 doanh thu chỉ có 1 nhân viên
    }
}