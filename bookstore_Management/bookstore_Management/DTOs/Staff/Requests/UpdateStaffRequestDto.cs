using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Staff.Requests
{
    /// <summary>
    /// DTO for updating an existing staff member
    /// </summary>
    public class UpdateStaffRequestDto
    {
        public string Name { get; set; }
        public UserRole? UserRole { get; set; }
    }
}

