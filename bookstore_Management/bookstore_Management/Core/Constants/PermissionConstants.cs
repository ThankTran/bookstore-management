using bookstore_Management.Core.Enums;

namespace bookstore_Management.Core.Constants
{
    /// <summary>
    /// Phân quyền cho từng role thành viên
    /// </summary>
    public class PermissionConstants
    {
        /// <summary>
        /// Quyền truy cập Quản Lý Sách
        /// </summary>
        public static class BookManagement
        {
            public static readonly Role[] View =
            {
                Role.Administrator, Role.SalesManager, Role.SalesStaff,
                Role.InventoryManager, Role.CustomerManager
            };

            public static readonly Role[] Create =
            {
                Role.Administrator, Role.InventoryManager
            };

            public static readonly Role[] Edit =
            {
                Role.Administrator, Role.InventoryManager
            };

            public static readonly Role[] Delete =
            {
                Role.Administrator
            };
        }

        /// <summary>
        /// Quyền truy cập Quản Lý Hóa Đơn
        /// </summary>
        public static class BillingManagement
        {
            public static readonly Role[] View =
            {
                Role.Administrator, Role.SalesManager, Role.SalesStaff,
                Role.InventoryManager, Role.CustomerManager
            };

            public static readonly Role[] Create =
            {
                Role.Administrator, Role.SalesManager, Role.SalesStaff
            };

            public static readonly Role[] Edit =
            {
                Role.Administrator, Role.SalesManager
            };

            public static readonly Role[] Delete =
            {
                Role.Administrator
            };
        }

        /// <summary>
        /// Quyền truy cập Quản Lý Kho
        /// </summary>
        public static class StockManagement
        {
            public static readonly Role[] View =
            {
                Role.Administrator, Role.SalesManager, Role.SalesStaff,
                Role.InventoryManager
            };

            public static readonly Role[] Create =
            {
                Role.Administrator, Role.InventoryManager
            };

            public static readonly Role[] Edit =
            {
                Role.Administrator, Role.InventoryManager
            };

            // THÊM: Delete quyền cho Stock
            public static readonly Role[] Delete =
            {
                Role.Administrator
            };
        }

        /// <summary>
        /// Quyền truy cập Quản Lý Nhân Viên
        /// </summary>
        public static class EmployeeManagement
        {
            public static readonly Role[] View =
            {
                Role.Administrator, Role.SalesManager
            };

            public static readonly Role[] Create =
            {
                Role.Administrator
            };

            public static readonly Role[] Edit =
            {
                Role.Administrator
            };

            public static readonly Role[] Delete =
            {
                Role.Administrator
            };
        }

        /// <summary>
        /// Quyền truy cập Quản Lý Khách Hàng
        /// </summary>
        public static class CustomerManagement
        {
            public static readonly Role[] View =
            {
                Role.Administrator, Role.SalesManager, Role.SalesStaff,
                Role.CustomerManager, Role.InventoryManager
            };

            public static readonly Role[] Create =
            {
                Role.Administrator, Role.SalesManager, Role.SalesStaff,
                Role.CustomerManager
            };

            public static readonly Role[] Edit =
            {
                Role.Administrator, Role.SalesManager, Role.CustomerManager
            };

            // THÊM: Delete quyền cho Customer
            public static readonly Role[] Delete =
            {
                Role.Administrator
            };
        }

        /// <summary>
        /// Quyền truy cập Báo Cáo Doanh Thu
        /// </summary>
        public static class ReportManagement
        {
            public static readonly Role[] View =
            {
                Role.Administrator, Role.SalesManager, Role.SalesStaff,
                Role.InventoryManager, Role.CustomerManager
            };

            public static readonly Role[] Export =
            {
                Role.Administrator, Role.SalesManager
            };
        }
    }

}