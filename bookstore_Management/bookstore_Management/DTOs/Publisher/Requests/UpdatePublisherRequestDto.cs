namespace bookstore_Management.DTOs.Supplier.Requests
{
    /// <summary>
    /// DTO for updating an existing supplier
    /// </summary>
    public class UpdateSupplierRequestDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}

