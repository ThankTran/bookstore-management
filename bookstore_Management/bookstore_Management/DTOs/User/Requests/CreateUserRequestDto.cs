namespace bookstore_Management.DTOs.User.Requests
{
    /// <summary>
    /// DTO for creating a new user
    /// </summary>
    public class CreateUserRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string StaffId { get; set; }
    }
}

