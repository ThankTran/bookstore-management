using System;

namespace bookstore_Management.DTOs
{
    public class WarehouseCreateDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class WarehouseUpdateDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class WarehouseResponseDto
    {
        public string WarehouseId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalBooks { get; set; }
        public int TotalQuantity { get; set; }
    }
}