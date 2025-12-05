using System.ComponentModel.DataAnnotations;

namespace bookstore_Management.DTOs
{
    /// <summary>
    /// DTO cho thêm/sửa nhà cung cấp
    /// </summary>
    public class SupplierDto
    {
        public string Name { get; set; } 
        public string Phone { get; set; }
        public string Address { get; set; }
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