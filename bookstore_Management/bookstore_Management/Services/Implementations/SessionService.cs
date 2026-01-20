using bookstore_Management.DTOs.User.Response;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Services
{
    public class SessionService
    {
        private static SessionService _instance;
        // Cách viết dự phòng cho C# cũ
        public static SessionService Instance
        {
            get
            {
                if (_instance == null) _instance = new SessionService();
                return _instance;
            }
        }
        // Lưu thông tin User sau khi đăng nhập thành công
        // Dùng UserResponseDto vì Service trả về DTO
        public UserResponseDto CurrentUser { get; private set; }

        public void StartSession(UserResponseDto user)
        {
            CurrentUser = user;
        }

        public void EndSession()
        {
            CurrentUser = null;
        }

        // Helper để check quyền nhanh
        public bool IsAdmin => CurrentUser?.Role == UserRole.Administrator;
        public bool IsSStaff => CurrentUser?.Role == UserRole.SalesStaff;
        public bool IsSManager => CurrentUser?.Role == UserRole.SalesManager;
        public bool IsIManager => CurrentUser?.Role == UserRole.InventoryManager;
        public bool IsCManager => CurrentUser?.Role == UserRole.CustomerManager;
    }
}