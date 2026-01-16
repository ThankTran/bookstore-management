namespace bookstore_Management.DTOs.Stock.Requests
{
    /// <summary>
    /// DTO for updating stock quantity
    /// </summary>
    public class UpdateStockRequestDto
    {
        public int BookId { get; set; }
        public int StockQuantity { get; set; }
    }
}

