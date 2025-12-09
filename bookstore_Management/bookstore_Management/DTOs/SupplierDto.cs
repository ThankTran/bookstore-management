using System.ComponentModel.DataAnnotations;

namespace bookstore_Management.DTOs
{
    /// <summary>
    /// DTO cho thêm/sửa nhà cung cấp
    /// </summary>
    public class SupplierDto
    {
        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
    }

    /// <summary>
    /// DTO cho tìm kiếm NCC
    /// </summary>
    public class SupplierSearchDto
    {
        public string Keyword { get; set; }
    }
}