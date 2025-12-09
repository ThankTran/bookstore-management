using System.ComponentModel.DataAnnotations;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs
{
    /// <summary>
    /// DTO cho thêm/sửa nhân viên
    /// </summary>
    public class StaffDto
    {
        [Required(ErrorMessage = "Tên nhân viên không được để trống")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Lương cơ bản không được để trống")]
        [Range(0, double.MaxValue)]
        public decimal BaseSalary { get; set; }

        [Required(ErrorMessage = "CCCD không được để trống")]
        [StringLength(12, MinimumLength = 9)]
        public string CitizenIdCard { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        public StaffStatus Status { get; set; } = StaffStatus.Working;
        public Role Role { get; set; } = Role.SalesStaff;

        [Range(0, double.MaxValue)]
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

        [StringLength(500)]
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
        [Required]
        [StringLength(6)]
        public string StaffId { get; set; }

        [Required]
        [Range(1, 31)]
        public int WorkingDays { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Bonus { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal Deduction { get; set; } = 0;
    }
}