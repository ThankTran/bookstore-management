using System.Linq;
using bookstore_Management.Core.Constants;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Core.Utils
{
    /// <summary>
    /// Utility class để check phân quyền trong hệ thống
    /// </summary>
    public class PermissionHelper
    {
        
        /// <summary>
        /// Kiểm tra quyền xem của user với feature
        /// </summary>
        public static bool CanView(UserRole userRole, string feature)
        {
            switch (feature)
            {
                case "Book":
                    return PermissionConstants.BookManagement.View.Contains(userRole);
                case "Billing":
                    return PermissionConstants.BillingManagement.View.Contains(userRole);
                case "Stock":
                    return PermissionConstants.StockManagement.View.Contains(userRole);
                case "Employee":
                    return PermissionConstants.EmployeeManagement.View.Contains(userRole);
                case "Customer":
                    return PermissionConstants.CustomerManagement.View.Contains(userRole);
                case "Report":
                    return PermissionConstants.ReportManagement.View.Contains(userRole);
                default:
                    return false;
            }
        }

        
        /// <summary>
        /// Kiểm tra quyền tạo mới của user với feature
        /// </summary>
        public static bool CanCreate(UserRole userRole, string feature)
        {
            switch (feature)
            {
                case "Book":
                    return PermissionConstants.BookManagement.Create.Contains(userRole);
                case "Billing":
                    return PermissionConstants.BillingManagement.Create.Contains(userRole);
                case "Stock":
                    return PermissionConstants.StockManagement.Create.Contains(userRole);
                case "Employee":
                    return PermissionConstants.EmployeeManagement.Create.Contains(userRole);
                case "Customer":
                    return PermissionConstants.CustomerManagement.Create.Contains(userRole);
                default:
                    return false;
            }
        }

        
        /// <summary>
        /// Kiểm tra quyền chỉnh sửa của user với feature
        /// </summary>
        public static bool CanEdit(UserRole userRole, string feature)
        {
            switch (feature)
            {
                case "Book":
                    return PermissionConstants.BookManagement.Edit.Contains(userRole);
                case "Billing":
                    return PermissionConstants.BillingManagement.Edit.Contains(userRole);
                case "Stock":
                    return PermissionConstants.StockManagement.Edit.Contains(userRole);
                case "Employee":
                    return PermissionConstants.EmployeeManagement.Edit.Contains(userRole);
                case "Customer":
                    return PermissionConstants.CustomerManagement.Edit.Contains(userRole);
                default:
                    return false;
            }
        }

        
        /// <summary>
        /// Kiểm tra quyền xóa của user với feature
        /// </summary>
        public static bool CanDelete(UserRole userRole, string feature)
        {
            switch (feature)
            {
                case "Book":
                    return PermissionConstants.BookManagement.Delete.Contains(userRole);
                case "Billing":
                    return PermissionConstants.BillingManagement.Delete.Contains(userRole);
                case "Employee":
                    return PermissionConstants.EmployeeManagement.Delete.Contains(userRole);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Kiểm tra quyền export của user với feature
        /// </summary>
        public static bool CanExport(UserRole userRole, string feature)
        {
            switch (feature)
            {
                case "Report":
                    return PermissionConstants.ReportManagement.Export.Contains(userRole);
                default:
                    return false;
            }
        }
        
    }
}
