using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookstore_Management.Core.Constants
{
    internal class MessageConstants
    {
        // Thành công
        public const string CreatedSuccess = "Thêm mới thành công";
        public const string UpdatedSuccess = "Cập nhật thành công";
        public const string DeletedSuccess = "Xóa thành công";
        public const string SavedSuccess = "Lưu thành công";
        public const string OpenSuccess = "Mở thành công";
        public const string Success = "Thành công";

        // Lỗi
        public const string Error = "Đã xảy ra lỗi";
        public const string NotFound = "Không tìm thấy dữ liệu";
        public const string Unauthorized = "Không có quyền truy cập";
        public const string InvalidData = "Dữ liệu không hợp lệ";

        // Cảnh báo
        public const string ConfirmDelete = "Bạn có chắc muốn xóa?";
        public const string UnsavedChanges = "Có thay đổi chưa lưu. Bạn có muốn tiếp tục?";

        // Validation
        public const string Required = "Mục này là bắt buộc";
        public const string InvalidFormat = "Định dạng không hợp lệ";

        // Business
        public const string OutOfStock = "Sản phẩm đã hết hàng";
        public const string InsufficientStock = "Số lượng trong kho không đủ";
        public const string Pending = "Đang xử lý";
        public const string Cancelled = "Đã hủy";
    }
}
