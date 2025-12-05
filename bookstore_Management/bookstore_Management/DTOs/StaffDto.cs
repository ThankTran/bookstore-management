using System.ComponentModel.DataAnnotations;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs
{
    /// <summary>
    /// DTO cho thêm/sửa nhân viên
    /// </summary>
    public class StaffDto
    {

        public string Name { get; set; } 
        public decimal BaseSalary { get; set; }
        public string CitizenIdCard { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public StaffStatus Status { get; set; } = StaffStatus.Working;
        public Role Role { get; set; } = Role.SalesStaff;
        public decimal SalaryRate { get; set; } = 1.0m;
    }

    /// <summary>
    /// DTO cho cập nhật trạng thái nhân viên
    /// </summary>
    public class UpdateStaffStatusDto
    {
        [Required]
        [StringLength(6)]
        public string StaffId { get; set; }

        [Required]
        public StaffStatus NewStatus { get; set; }

        public string Reason { get; set; }
    }

    /// <summary>
    /// DTO cho tìm kiếm nhân viên
    /// </summary>
    public class StaffSearchDto
    {
        public string Keyword { get; set; }
        public Role? Role { get; set; }
        public StaffStatus? Status { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
    }

    /// <summary>
    /// DTO cho tính lương
    /// </summary>
    public class CalculateSalaryDto
    {

        public string StaffId { get; set; }
        public int WorkingDays { get; set; }
        public decimal Bonus { get; set; } = 0;
        public decimal Deduction { get; set; } = 0;
    }
}