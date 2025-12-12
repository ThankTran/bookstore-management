using System;
using System.Text;
using bookstore_Management.Models;

namespace bookstore_Management.Helpers
{
    internal static class PrintHelper
    {
        public static string BuildOrderReceipt(Order order)
        {
            if (order == null) return string.Empty;

            // In ra hóa đơn dạng text đơn giản, tránh phụ thuộc framework in ấn
            var sb = new StringBuilder();
            sb.AppendLine($"Đơn hàng: {order.OrderId}");
            sb.AppendLine($"Nhân viên: {order.StaffId}");
            sb.AppendLine($"Khách hàng: {order.CustomerId ?? "Vãng lai"}");
            sb.AppendLine($"Ngày: {order.CreatedDate:dd/MM/yyyy HH:mm}");
            sb.AppendLine("----------------------------------------");

            if (order.OrderDetails != null)
            {
                foreach (var d in order.OrderDetails)
                {
                    sb.AppendLine($"{d.BookId} x{d.Quantity} @ {d.SalePrice:N0} = {d.Subtotal:N0}");
                }
            }

            sb.AppendLine("----------------------------------------");
            sb.AppendLine($"Giảm giá: {order.Discount:N0}");
            sb.AppendLine($"Tổng cộng: {order.TotalPrice:N0}");
            if (!string.IsNullOrWhiteSpace(order.Notes))
                sb.AppendLine($"Ghi chú: {order.Notes}");

            return sb.ToString();
        }
    }
}
