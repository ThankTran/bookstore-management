namespace bookstore_Management.DTOs.Order.Requests
{
    /// <summary>
    /// DTO for creating order detail item
    /// </summary>
    public class OrderDetailCreateRequestDto
    {
        public string BookId { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
    }
}

