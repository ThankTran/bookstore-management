namespace bookstore_Management.DTOs.Order.Requests
{
    
    public class OrderDetailCreateRequestDto
    {
        public string BookId { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
    }
}

