namespace bookstore_Management.DTOs.Warehouse.Requests
{
    /// <summary>
    /// DTO for updating an existing warehouse
    /// </summary>
    public class UpdateWarehouseRequestDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}

