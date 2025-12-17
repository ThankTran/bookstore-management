using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Core.Constants
{
    /// <summary>
    /// Hằng số cho role
    /// </summary>
    public static class RoleConstants
    {
        /// <summary>
        /// MAPPING ENUM -> STRING (tự động generate từ Enum)
        /// </summary>
        public static readonly Dictionary<UserRole, string> Name = GenerateRoleNameMapping();

        private static Dictionary<UserRole, string> GenerateRoleNameMapping()
        {
            return Enum.GetValues(typeof(UserRole))
                .Cast<UserRole>()
                .ToDictionary(r => r, r => r.ToString());
        }

        /// <summary>
        /// Role mặc định khi tạo nhân viên mới
        /// </summary>
        public const UserRole DefaultRole = UserRole.SalesStaff;

        /// <summary>
        /// Lấy tên role (string) từ enum
        /// </summary>
        public static string GetRoleName(UserRole userRole) => userRole.ToString();

        /// <summary>
        /// Lấy tên hiển thị tiếng Việt của role
        /// </summary>
        public static string GetRoleDisplayName(UserRole userRole) => userRole.GetDisplayName();
    }
}
