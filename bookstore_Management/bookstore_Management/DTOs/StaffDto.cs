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
        [StringLength(50, ErrorMessage = "Tên không quá 50 ký tự")]
        public string Name { get; set; } 
        
        [Required(ErrorMessage = "Lương cứng không được để trống")]
        [Range(3000000, 50000000, ErrorMessage = "Lương phải từ 3,000,000 đến 50,000,000")]
        public decimal BaseSalary { get; set; }
        
        [Required(ErrorMessage = "CCCD không được để trống")]
        [StringLength(10, MinimumLength = 9, ErrorMessage = "CCCD phải 9-10 số")]
        [RegularExpression(@"^\d+$", ErrorMessage = "CCCD chỉ được chứa số")]
        public string CitizenIdCard { get; set; }
        
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "SĐT phải đúng 10 số")]
        [RegularExpression(@"^\d+$", ErrorMessage = "SĐT chỉ được chứa số")]
        public string Phone { get; set; }
        
        [StringLength(50, ErrorMessage = "Địa chỉ không quá 50 ký tự")]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Chưa chọn trạng thái")]
        public StaffStatus Status { get; set; }

        [Required(ErrorMessage = "Chưa chọn vai trò")]
        public Role Role { get; set; } = Role.SalesStaff;

        [Required(ErrorMessage = "Chưa nhập hệ số lương")]
        [Range(0.1, 10.0, ErrorMessage = "Hệ số lương phải từ 0.1 đến 10.0")]
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