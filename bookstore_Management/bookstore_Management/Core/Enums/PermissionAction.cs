using System.ComponentModel.DataAnnotations;

namespace bookstore_Management.Core.Enums
{
    /// <summary>
    /// Các hành động có thể phân quyền
    /// </summary>
    public enum PermissionAction
    {
        [Display(Name = "Xem")]
        View = 1,

        [Display(Name = "Tạo mới")]
        Create = 2,

        [Display(Name = "Chỉnh sửa")]
        Edit = 3,

        [Display(Name = "Xóa")]
        Delete = 4,

        [Display(Name = "Xuất")]
        Export = 5
    }
}