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
        private static readonly Dictionary<Feature, Type> FeatureMap = InitializeFeatureMap();
        private static Dictionary<Feature, Type> InitializeFeatureMap()
        {
            var map = new Dictionary<Feature, Type>();
             map.Add(Feature.Book, typeof(PermissionConstants.ReportView));
             map.Add(Feature.Invoices, typeof(PermissionConstants.InvoiceView));
             map.Add(Feature.Publisher, typeof(PermissionConstants.PublisherView));
             map.Add(Feature.Staff, typeof(PermissionConstants.StaffView)); 
             map.Add(Feature.Customer, typeof(PermissionConstants.CustomerManageView));
             map.Add(Feature.Report, typeof(PermissionConstants.ReportView));
            return map;
        }

        /// <summary>
        /// Kiểm tra quyền của user với feature và action
        /// </summary>
        private static bool HasPermission(UserRole userRole, Feature feature, string action)
        {
            // Kiểm tra feature có tồn tại không
            if (!FeatureMap.TryGetValue(feature, out var featureType))
                return false;

            // Lấy field theo tên action (View, Create, Edit, Delete, Export)
            var field = featureType.GetField(action,
                BindingFlags.Public | BindingFlags.Static);

            // Nếu field không tồn tại hoặc không phải Role[], return false
            var permissions = field?.GetValue(null) as UserRole[];
            if (permissions == null)
                return false;

            return permissions.Contains(userRole);
        }
        


        /// <summary>
        /// Kiểm tra quyền xem của user với feature
        /// </summary>
        public static bool CanView(UserRole userRole, Feature feature)
            => HasPermission(userRole, feature, nameof(PermissionAction.View));

        /// <summary>
        /// Kiểm tra quyền tạo mới của user với feature
        /// </summary>
        public static bool CanCreate(UserRole userRole, Feature feature)
            => HasPermission(userRole, feature, nameof(PermissionAction.Create));

        /// <summary>
        /// Kiểm tra quyền chỉnh sửa của user với feature
        /// </summary>
        public static bool CanEdit(UserRole userRole, Feature feature)
            => HasPermission(userRole, feature, nameof(PermissionAction.Edit));

        /// <summary>
        /// Kiểm tra quyền xóa của user với feature
        /// </summary>
        public static bool CanDelete(UserRole userRole, Feature feature)
            => HasPermission(userRole, feature, nameof(PermissionAction.Delete));

        /// <summary>
        /// Kiểm tra quyền export của user với feature
        /// </summary>
        public static bool CanExport(UserRole userRole, Feature feature)
            => HasPermission(userRole, feature, nameof(PermissionAction.Export));

        /// <summary>
        /// Kiểm tra quyền generic (nếu cần)
        /// </summary>
        public static bool CanPerform(UserRole userRole, Feature feature, PermissionAction action)
            => HasPermission(userRole, feature, action.ToString());
    }
}
