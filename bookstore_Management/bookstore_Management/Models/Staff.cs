using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Models
{
    /// <summary>
    /// Thông tin cần quản lý của nhân viên
    /// </summary>
    public class Staff
    {
        // mã nhân viên - khóa chính
        [Column("id")]
        [StringLength(6)]
        public string Id { get; set; }

        // tên nhân viên
        [Required]
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } 
        
        // lương cứng
        [Required]
        [Column("base_salary")]
        public decimal BaseSalary { get; set; }
        
        // căn cước công dân
        [Required]
        [Column("citizen_id_card")]
        [StringLength(10)]
        public string CitizenIdCard { get; set; }
        
        // số điện thoại - có thể null
        [Required]
        [Column("phone")]
        [StringLength(10)]
        public string Phone { get; set; }
        
        // địa chỉ - có thể null
        [Column("address")]
        [StringLength(50)]
        public string Address { get; set; }
        
        // trạng thái nhân viên
        [Required]
        [Column("status")]
        public StaffStatus Status { get; set; }
        
        // vai trò
        [Required]
        [Column("role")]
        public Role Role { get; set; }
        
        // hệ số lương
        [Required]
        [Column("salary_rate")]
        public decimal SalaryRate { get; set; }
        
        // navigation properties
        public virtual ICollection<StaffDailyRevenue> DailyRevenues { get; set; }
    }
}