using System.ComponentModel.DataAnnotations;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs
{
    /// <summary>
    /// DTO cho thêm/sửa khách hàng
    /// </summary>
    public class CustomerDto
    {
        public string Name { get; set; } 
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
    
    /// <summary>
    /// DTO cho tìm kiếm khách hàng
    /// </summary>
    public class CustomerSearchDto
    {
        public string Keyword { get; set; }
        public MemberTier? MemberLevel { get; set; }
        public decimal? MinPoints { get; set; }
        public decimal? MaxPoints { get; set; }
    }


}