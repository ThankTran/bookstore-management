namespace bookstore_Management.DTOs.Book.Shared
{
    /// <summary>
    /// Shared DTO for stock information by warehouse
    /// Used in Book-related responses
    /// </summary>
    public class StockInfoDto
    {
        public string WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int Quantity { get; set; }
    }
}

