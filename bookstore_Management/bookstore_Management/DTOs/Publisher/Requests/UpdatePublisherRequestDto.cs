namespace bookstore_Management.DTOs.Publisher.Requests
{
    /// <summary>
    /// DTO for updating an existing supplier
    /// </summary>
    public class UpdatePublisherRequestDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}

