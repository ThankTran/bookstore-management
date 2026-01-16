namespace bookstore_Management.DTOs.Publisher.Requests
{
    /// <summary>
    /// DTO for creating a new supplier
    /// </summary>
    public class CreatePublisherRequestDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}

