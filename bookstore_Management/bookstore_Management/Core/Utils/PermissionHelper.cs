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
        public static bool CanView(Role role, string feature)
        {
            switch (feature)
            {
                case "Book":
                    return PermissionConstants.BookManagement.View.Contains(role);
                case "Billing":
                    return PermissionConstants.BillingManagement.View.Contains(role);
                case "Stock":
                    return PermissionConstants.StockManagement.View.Contains(role);
                case "Employee":
                    return PermissionConstants.EmployeeManagement.View.Contains(role);
                case "Customer":
                    return PermissionConstants.CustomerManagement.View.Contains(role);
                case "Report":
                    return PermissionConstants.ReportManagement.View.Contains(role);
                default:
                    return false;
            }
        }

        
        /// <summary>
        /// Kiểm tra quyền tạo mới của user với feature
        /// </summary>
        public static bool CanCreate(Role role, string feature)
        {
            switch (feature)
            {
                case "Book":
                    return PermissionConstants.BookManagement.Create.Contains(role);
                case "Billing":
                    return PermissionConstants.BillingManagement.Create.Contains(role);
                case "Stock":
                    return PermissionConstants.StockManagement.Create.Contains(role);
                case "Employee":
                    return PermissionConstants.EmployeeManagement.Create.Contains(role);
                case "Customer":
                    return PermissionConstants.CustomerManagement.Create.Contains(role);
                default:
                    return false;
            }
        }

        
        /// <summary>
        /// Kiểm tra quyền chỉnh sửa của user với feature
        /// </summary>
        public static bool CanEdit(Role role, string feature)
        {
            switch (feature)
            {
                case "Book":
                    return PermissionConstants.BookManagement.Edit.Contains(role);
                case "Billing":
                    return PermissionConstants.BillingManagement.Edit.Contains(role);
                case "Stock":
                    return PermissionConstants.StockManagement.Edit.Contains(role);
                case "Employee":
                    return PermissionConstants.EmployeeManagement.Edit.Contains(role);
                case "Customer":
                    return PermissionConstants.CustomerManagement.Edit.Contains(role);
                default:
                    return false;
            }
        }

        
        /// <summary>
        /// Kiểm tra quyền xóa của user với feature
        /// </summary>
        public static bool CanDelete(Role role, string feature)
        {
            switch (feature)
            {
                case "Book":
                    return PermissionConstants.BookManagement.Delete.Contains(role);
                case "Billing":
                    return PermissionConstants.BillingManagement.Delete.Contains(role);
                case "Employee":
                    return PermissionConstants.EmployeeManagement.Delete.Contains(role);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Kiểm tra quyền export của user với feature
        /// </summary>
        public static bool CanExport(Role role, string feature)
        {
            switch (feature)
            {
                case "Report":
                    return PermissionConstants.ReportManagement.Export.Contains(role);
                default:
                    return false;
            }
        }
        
    }
}
