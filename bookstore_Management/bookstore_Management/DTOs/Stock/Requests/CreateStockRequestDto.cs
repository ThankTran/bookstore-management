namespace bookstore_Management.DTOs.Stock.Requests
{
    /// <summary>
    /// DTO for creating stock entry
    /// </summary>
    public class CreateStockRequestDto
    {
        public string WarehouseId { get; set; }
        public string BookId { get; set; }
        public int StockQuantity { get; set; }
    }
}

