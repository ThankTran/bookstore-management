namespace bookstore_Management.DTOs.Order.Responses
{
    /// <summary>
    /// DTO for order detail response
    /// </summary>
    public class OrderDetailResponseDto
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public string Notes { get; set; }
    }
}

