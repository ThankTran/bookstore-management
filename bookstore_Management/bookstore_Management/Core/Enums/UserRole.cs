using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace bookstore_Management.Core.Enums
{
    /// <summary>
    /// Các role được gán số để sử dụng
    /// Sử dụng Display attribute để ra tên tiếng việt
    /// </summary>
    public enum UserRole
    {
        Unknown = 0,
        
        [Display(Name = "Quản trị viên")]
        Administrator = 1,

        [Display(Name = "Quản lý bán hàng")]
        SalesManager = 2,

        [Display(Name = "Nhân viên bán hàng")]
        SalesStaff = 3,

        [Display(Name = "Quản lý kho")]
        InventoryManager = 4,

        [Display(Name = "Quản lý khách hàng")]
        CustomerManager = 5
    }

    /// <summary>
    /// Extension methods cho Enum
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Lấy Display Name từ enum
        /// </summary>
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        /// <summary>
        /// Lấy Description từ enum (deprecated - sử dụng GetDisplayName)
        /// </summary>
        [Obsolete("Use GetDisplayName instead")]
        public static string GetDescription(this Enum value)
            => value.GetDisplayName();
    }
}
