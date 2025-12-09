using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using bookstore_Management.Core.Constants;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Core.Utils
{
    /// <summary>
    /// Utility class để check phân quyền trong hệ thống
    /// </summary>
    public class PermissionHelper
    {
        private static readonly Dictionary<string, Type> FeatureMap = InitializeFeatureMap();
        private static Dictionary<string, Type> InitializeFeatureMap()
        {
            var map = new Dictionary<string, Type>();
            map.Add(FeatureConstants.Book, typeof(PermissionConstants.BookManagement));
            map.Add(FeatureConstants.Billing, typeof(PermissionConstants.BillingManagement));
            map.Add(FeatureConstants.Stock, typeof(PermissionConstants.StockManagement));
            map.Add(FeatureConstants.Employee, typeof(PermissionConstants.EmployeeManagement));
            map.Add(FeatureConstants.Customer, typeof(PermissionConstants.CustomerManagement));
            map.Add(FeatureConstants.Report, typeof(PermissionConstants.ReportManagement));
            return map;
        }

        /// <summary>
        /// Kiểm tra quyền của user với feature và action
        /// </summary>
        private static bool HasPermission(Role role, string feature, string action)
        {
            // Kiểm tra feature có tồn tại không
            if (!FeatureMap.TryGetValue(feature, out var featureType))
                return false;

            // Lấy field theo tên action (View, Create, Edit, Delete, Export)
            var field = featureType.GetField(action,
                BindingFlags.Public | BindingFlags.Static);

            // Nếu field không tồn tại hoặc không phải Role[], return false
            var permissions = field?.GetValue(null) as Role[];
            if (permissions == null)
                return false;

            return permissions.Contains(role);
        }

        /// <summary>
        /// Kiểm tra quyền xem của user với feature
        /// </summary>
        public static bool CanView(Role role, string feature)
            => HasPermission(role, feature, nameof(PermissionAction.View));

        /// <summary>
        /// Kiểm tra quyền tạo mới của user với feature
        /// </summary>
        public static bool CanCreate(Role role, string feature)
            => HasPermission(role, feature, nameof(PermissionAction.Create));

        /// <summary>
        /// Kiểm tra quyền chỉnh sửa của user với feature
        /// </summary>
        public static bool CanEdit(Role role, string feature)
            => HasPermission(role, feature, nameof(PermissionAction.Edit));

        /// <summary>
        /// Kiểm tra quyền xóa của user với feature
        /// </summary>
        public static bool CanDelete(Role role, string feature)
            => HasPermission(role, feature, nameof(PermissionAction.Delete));

        /// <summary>
        /// Kiểm tra quyền export của user với feature
        /// </summary>
        public static bool CanExport(Role role, string feature)
            => HasPermission(role, feature, nameof(PermissionAction.Export));

        /// <summary>
        /// Kiểm tra quyền generic (nếu cần)
        /// </summary>
        public static bool CanPerform(Role role, string feature, PermissionAction action)
            => HasPermission(role, feature, action.ToString());
    }
}
