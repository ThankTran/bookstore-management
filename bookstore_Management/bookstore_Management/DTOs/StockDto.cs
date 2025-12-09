using System.ComponentModel.DataAnnotations;

namespace bookstore_Management.DTOs
{
    /// <summary>
    /// DTO cho cập nhật tồn kho
    /// </summary>
    public class StockDto
    {
        [Required(ErrorMessage = "Mã sách không được để trống")]
        [StringLength(6)]
        public string BookId { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
    }

    /// <summary>
    /// DTO cho thêm/trừ tồn kho
    /// </summary>
    public class AdjustStockDto
    {
        [Required(ErrorMessage = "Mã sách không được để trống")]
        [StringLength(6)]
        public string BookId { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải >= 1")]
        public int Quantity { get; set; }
    }
}