using System.ComponentModel;

namespace bookstore_Management.Core.Enums
{
    /// <summary>
    /// Các role được gán số để sử dụng
    /// có thể sử dụng .Description để ra tên tiếng việt
    /// </summary>
    public enum UserRole
    {
        [Description("Quản trị viên")]
        Administrator = 1,
    
        [Description("Quản lý bán hàng")]
        SalesManager = 2,
    
        [Description("Nhân viên bán hàng")]
        SalesStaff = 3,
    
        [Description("Quản lý kho")]
        InventoryManager = 4,
    
        [Description("Quản lý khách hàng")]
        CustomerManager = 5
    }
}
