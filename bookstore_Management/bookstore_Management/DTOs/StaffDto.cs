using System;
using System.ComponentModel.DataAnnotations;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs
{
    // ==================== STAFF DTOs ====================
    
    public class StaffCreateDto
    {
        public string Name { get; set; }
        public UserRole UserRole { get; set; }
    }

    public class StaffUpdateDto
    {
        public string Name { get; set; }
        public UserRole? UserRole { get; set; }
    }

    public class StaffResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public UserRole UserRole { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalOrders { get; set; }
    }
}