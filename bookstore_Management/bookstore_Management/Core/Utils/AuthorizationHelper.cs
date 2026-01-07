using bookstore_Management.Core.Enums;

namespace bookstore_Management.Core.Utils
{
    /// <summary>
    /// Helper class for role-based authorization
    /// Enforces authorization at service level
    /// </summary>
    public static class AuthorizationHelper
    {
        /// <summary>
        /// Checks if user role has full access (Administrator)
        /// </summary>
        public static bool IsAdmin(UserRole role)
        {
            return role == UserRole.Administrator;
        }

        /// <summary>
        /// Checks if user role has limited access (Staff roles)
        /// </summary>
        public static bool IsStaff(UserRole role)
        {
            return role == UserRole.SalesManager ||
                   role == UserRole.SalesStaff ||
                   role == UserRole.InventoryManager ||
                   role == UserRole.CustomerManager;
        }

        /// <summary>
        /// Checks if user role has read-only access
        /// </summary>
        public static bool IsReadOnly(UserRole role)
        {
            // Currently all roles have write access except if explicitly restricted
            // This can be extended for future read-only roles
            return false;
        }

        /// <summary>
        /// Checks if user can perform admin operations
        /// </summary>
        public static bool CanPerformAdminOperation(UserRole role)
        {
            return IsAdmin(role);
        }

        /// <summary>
        /// Checks if user can manage staff
        /// </summary>
        public static bool CanManageStaff(UserRole role)
        {
            return IsAdmin(role) || role == UserRole.SalesManager;
        }

        /// <summary>
        /// Checks if user can manage inventory
        /// </summary>
        public static bool CanManageInventory(UserRole role)
        {
            return IsAdmin(role) || role == UserRole.InventoryManager;
        }

        /// <summary>
        /// Checks if user can manage customers
        /// </summary>
        public static bool CanManageCustomers(UserRole role)
        {
            return IsAdmin(role) || role == UserRole.CustomerManager || role == UserRole.SalesManager;
        }

        /// <summary>
        /// Checks if user can create orders
        /// </summary>
        public static bool CanCreateOrders(UserRole role)
        {
            return IsAdmin(role) || 
                   role == UserRole.SalesManager || 
                   role == UserRole.SalesStaff;
        }
    }
}

