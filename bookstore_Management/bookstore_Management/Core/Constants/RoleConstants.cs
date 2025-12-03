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
        public static readonly Dictionary<Role, string> Name = new Dictionary<Role, string>
        {
            [Role.Administrator] = "Administrator",
            [Role.SalesManager] = "SalesManager", 
            [Role.SalesStaff] = "SalesStaff",
            [Role.InventoryManager] = "InventoryManager",
            [Role.CustomerManager] = "CustomerManager"
        };

        

        
        /// <summary>
        /// role mặc định khi tạo nhân viên mới
        /// </summary>
        public const Role DefaultRole = Role.SalesStaff;
    }
}
