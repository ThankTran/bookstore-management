using bookstore_Management.Core.Enums;

namespace bookstore_Management.Models
{
    public static class SessionModel
    {
        public static string UserId { get; set; }
        public static string Username { get; set; }
        public static UserRole Role { get; set; }

        public static bool IsLoggedIn => !string.IsNullOrEmpty(UserId);

        public static void Clear()
        {
            UserId = null;
            Username = null;            
        }
        public static string RoleDisplay
        {
            get
            {
                switch (Role)
                {
                    case UserRole.Administrator:
                        return "Quản lý";
                    case UserRole.SalesStaff:
                        return "Nhân viên bán hàng";
                    case UserRole.SalesManager:
                        return "Quản lý bán hàng";
                    case UserRole.CustomerManager:
                        return "Quản lý khách hàng";
                    case UserRole.InventoryManager:
                        return "Quản lý kho";
                    default:
                        return "User";
                }
            }
        }



    }

}
