namespace bookstore_Management.DTOs.Customer.Requests
{
    /// <summary>
    /// DTO for creating a new customer
    /// </summary>
    public class CreateCustomerRequestDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}

