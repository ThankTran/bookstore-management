using System.ComponentModel.DataAnnotations;

namespace bookstore_Management.Core.Enums
{
    public enum StaffStatus
    {
        [Display(Name = "Vẫn đang làm việc")]
        Working = 0,
        
        [Display(Name = "Đã nghỉ việc")]
        Quit = 1,
        
        [Display(Name = "Trong kì nghỉ phép")]
        Rest = 2
    }
}