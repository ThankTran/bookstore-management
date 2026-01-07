namespace bookstore_Management.DTOs.ImportBill.Responses
{
    /// <summary>
    /// DTO for import bill detail response
    /// </summary>
    public class ImportBillDetailResponseDto
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}

