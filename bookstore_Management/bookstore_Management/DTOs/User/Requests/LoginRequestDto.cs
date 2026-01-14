namespace bookstore_Management.DTOs.User.Requests
{
    /// <summary>
    /// DTO for user login
    /// </summary>
    public class LoginRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

