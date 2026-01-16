using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Staff.Requests
{

    public class UpdateStaffRequestDto
    {
        public string Name { get; set; }
        public string CitizenId { get; set; }
        public string Phone { get; set; }
        public UserRole? UserRole { get; set; }
    }
}

