using System.ComponentModel.DataAnnotations;
using bookstore_Management.Core.Constants;

namespace bookstore_Management.Core.Enums
{
    public enum OrderStatus
    {
        [Display(Name = MessageConstants.Pending)]
        Pending = 1,
    
        [Display(Name = MessageConstants.Success)]
        Complete = 2,
    
        [Display(Name = MessageConstants.Error)]
        Error = 3,
        
        [Display(Name = MessageConstants.Cancelled)]
        Cancelled = 4,
    }
}