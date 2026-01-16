using System;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Staff.Responses
{

    public class StaffResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CitizenId { get; set; }
        public string Phone { get; set; }
        public UserRole UserRole { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalOrders { get; set; }
    }
}

