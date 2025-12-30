namespace bookstore_Management.DTOs.Auth
{
    /// <summary>
    /// DTO for login request
    /// Contains only username and password (plain text, will be hashed in service)
    /// </summary>
    public class LoginRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

