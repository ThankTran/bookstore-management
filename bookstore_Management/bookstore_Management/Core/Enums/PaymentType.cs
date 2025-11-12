using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookstore_Management.Core.Enums
{
    public enum PaymentType
    {
        [Description("Tiền mặt")]
        Cash = 1,
    
        [Description("Chuyển khoản / chuyển tiền ngân hàng")]
        BankTransfer = 2,
    
        [Description("Quẹt thẻ")]
        Card = 3,
    
        [Description("Thẻ tín dụng")]
        DebitCard = 4,
    }
}
