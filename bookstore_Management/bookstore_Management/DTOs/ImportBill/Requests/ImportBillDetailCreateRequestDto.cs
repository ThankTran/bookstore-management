namespace bookstore_Management.DTOs.ImportBill.Requests
{
    /// <summary>
    /// DTO for creating import bill detail item
    /// </summary>
    public class ImportBillDetailCreateRequestDto
    {
        public string BookId { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
    }
}

