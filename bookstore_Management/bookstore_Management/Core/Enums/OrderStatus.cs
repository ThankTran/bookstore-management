using System.ComponentModel;
using bookstore_Management.Core.Constants;

namespace bookstore_Management.Core.Enums
{
    public enum OrderStatus
    {
        [Description(MessageConstants.Pending)]
        Pending = 1,
    
        [Description(MessageConstants.Success)]
        Complete = 2,
    
        [Description(MessageConstants.Error)]
        Error = 3,
        
        [Description(MessageConstants.Cancelled)]
        Cancelled = 4,
    }
}
