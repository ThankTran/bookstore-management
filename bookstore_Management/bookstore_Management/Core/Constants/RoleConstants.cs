using System.Collections.Generic;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Core.Constants
{
    /// <summary>
    /// Hằng số cho role
    /// </summary>
    public static class RoleConstants
    {

        /// <summary>
        /// MAPPING ENUM -> STRING (dùng cho authentication)
        /// </summary>
        public static readonly Dictionary<UserRole, string> RoleNames = new Dictionary<UserRole, string>
        {
            [UserRole.Administrator] = "Administrator",
            [UserRole.SalesManager] = "SalesManager", 
            [UserRole.SalesStaff] = "SalesStaff",
            [UserRole.InventoryManager] = "InventoryManager",
            [UserRole.CustomerManager] = "CustomerManager"
        };

        
        /// <summary>
        /// MÔ TẬT CHỨC NĂNG TỪNG ROLE
        /// </summary>
        public static readonly Dictionary<UserRole, string> RoleDescriptions = new Dictionary<UserRole, string>
        {
            [UserRole.Administrator] = "Toàn quyền hệ thống, quản lý người dùng và cấu hình",
            [UserRole.SalesManager] = "Quản lý hoạt động bán hàng, doanh thu, chiết khấu",
            [UserRole.SalesStaff] = "Bán hàng trực tiếp, xử lý giao dịch, tư vấn khách",
            [UserRole.InventoryManager] = "Quản lý tồn kho, nhập/xuất hàng, kiểm kê",
            [UserRole.CustomerManager] = "Quản lý thông tin khách hàng, chăm sóc khách hàng"
        };

        
        /// <summary>
        /// ROLE MẶC ĐỊNH KHI TẠO USER MỚI
        /// </summary>
        public const UserRole DefaultRole = UserRole.SalesStaff;
    }
}
