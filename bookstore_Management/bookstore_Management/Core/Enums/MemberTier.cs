using System.ComponentModel.DataAnnotations;

namespace bookstore_Management.Core.Enums
{
    public enum MemberTier
    {
        [Display(Name = "Vãng lai")]
        WalkIn = 0,
        
        [Display(Name = "Thành viên đồng")]
        Bronze = 1,
        
        [Display(Name = "Thành viên bạc")]
        Silver = 2,
        
        [Display(Name = "Thành viên vàng")]
        Gold = 3,
        
        [Display(Name = "Thành viên kim cương")]
        Diamond = 4
    }
}