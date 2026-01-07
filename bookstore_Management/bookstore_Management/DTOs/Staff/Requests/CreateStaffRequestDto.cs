using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Staff.Requests
{
    /// <summary>
    /// DTO for creating a new staff member
    /// </summary>
    public class CreateStaffRequestDto
    {
        public string Name { get; set; }
        public UserRole UserRole { get; set; }
    }
}

