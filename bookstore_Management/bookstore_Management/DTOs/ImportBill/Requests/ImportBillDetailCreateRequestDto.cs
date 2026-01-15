namespace bookstore_Management.DTOs.ImportBill.Requests
{
    
    public class ImportBillDetailCreateRequestDto
    {
        public string BookId { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
    }
}

