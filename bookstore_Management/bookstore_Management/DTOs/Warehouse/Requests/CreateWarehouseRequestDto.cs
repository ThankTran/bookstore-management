namespace bookstore_Management.DTOs.Warehouse.Requests
{
    /// <summary>
    /// DTO for creating a new warehouse
    /// </summary>
    public class CreateWarehouseRequestDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}

