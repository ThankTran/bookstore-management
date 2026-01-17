namespace bookstore_Management.DTOs.Common.Reports
{
    public class CustomerPurchaseRatioDto
    {
        public int WalkIn { get; set; } // vãng lai
        public int Member { get; set; } // thành viên
        public double WalkInRatio { get; set; }
        public double MemberRatio { get; set; }
        
    }
}