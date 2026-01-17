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
        public static class BookView
        {
            public static readonly UserRole[] View =
            {
                UserRole.Administrator, UserRole.SalesManager, UserRole.SalesStaff,
                UserRole.InventoryManager, UserRole.CustomerManager
            };

            public static readonly UserRole[] Create =
            {
                UserRole.Administrator, UserRole.InventoryManager
            };

            public static readonly UserRole[] Edit =
            {
                UserRole.Administrator, UserRole.InventoryManager
            };

            public static readonly UserRole[] Delete =
            {
                UserRole.Administrator
            };
        }

        /// <summary>
        /// Quyền truy cập Quản Lý Hóa Đơn
        /// </summary>
        public static class InvoiceView
        {
            public static readonly UserRole[] View =
            {
                UserRole.Administrator, UserRole.SalesManager, UserRole.SalesStaff,
                UserRole.InventoryManager, UserRole.CustomerManager
            };

            public static readonly UserRole[] Create =
            {
                UserRole.Administrator, UserRole.SalesManager, UserRole.SalesStaff
            };

            public static readonly UserRole[] Edit =
            {
                UserRole.Administrator, UserRole.SalesManager
            };

            public static readonly UserRole[] Delete =
            {
                UserRole.Administrator
            };
        }

        /// <summary>
        /// Quyền truy cập Quản Lý Kho
        /// </summary>
        public static class PublisherView
        {
            public static readonly UserRole[] View =
            {
                UserRole.Administrator, UserRole.SalesManager, UserRole.SalesStaff,
                UserRole.InventoryManager
            };

            public static readonly UserRole[] Create =
            {
                UserRole.Administrator, UserRole.InventoryManager
            };

            public static readonly UserRole[] Edit =
            {
                UserRole.Administrator, UserRole.InventoryManager
            };

            // THÊM: Delete quyền cho Stock
            public static readonly UserRole[] Delete =
            {
                UserRole.Administrator
            };
        }

        /// <summary>
        /// Quyền truy cập Quản Lý Nhân Viên
        /// </summary>
        public static class StaffView
        {
            public static readonly UserRole[] View =
            {
                UserRole.Administrator, UserRole.SalesManager
            };

            public static readonly UserRole[] Create =
            {
                UserRole.Administrator
            };

            public static readonly UserRole[] Edit =
            {
                UserRole.Administrator
            };

            public static readonly UserRole[] Delete =
            {
                UserRole.Administrator
            };
        }

        /// <summary>
        /// Quyền truy cập Quản Lý Khách Hàng
        /// </summary>
        public static class CustomerManageView
        {
            public static readonly UserRole[] View =
            {
                UserRole.Administrator, UserRole.SalesManager, UserRole.SalesStaff,
                UserRole.CustomerManager, UserRole.InventoryManager
            };

            public static readonly UserRole[] Create =
            {
                UserRole.Administrator, UserRole.SalesManager, UserRole.SalesStaff,
                UserRole.CustomerManager
            };

            public static readonly UserRole[] Edit =
            {
                UserRole.Administrator, UserRole.SalesManager, UserRole.CustomerManager
            };

            // THÊM: Delete quyền cho Customer
            public static readonly UserRole[] Delete =
            {
                UserRole.Administrator
            };
        }

        /// <summary>
        /// Quyền truy cập Báo Cáo Doanh Thu
        /// </summary>
        public static class ReportView
        {
            public static readonly UserRole[] View =
            {
                UserRole.Administrator, UserRole.SalesManager, UserRole.SalesStaff,
                UserRole.InventoryManager, UserRole.CustomerManager
            };

            public static readonly UserRole[] Export =
            {
                UserRole.Administrator, UserRole.SalesManager
            };
        }
    }

}