namespace bookstore_Management.DTOs.Supplier.Requests
{
    /// <summary>
    /// DTO for creating a new supplier
    /// </summary>
    public class CreateSupplierRequestDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}

