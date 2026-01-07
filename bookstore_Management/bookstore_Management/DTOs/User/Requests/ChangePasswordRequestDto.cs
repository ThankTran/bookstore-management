namespace bookstore_Management.DTOs.User.Requests
{
    /// <summary>
    /// DTO for changing user password
    /// </summary>
    public class ChangePasswordRequestDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

