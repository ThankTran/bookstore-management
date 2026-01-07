using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace bookstore_Management.Presentation.Converters
{
    /// <summary>
    /// Converter chuyển đổi số lượng items thành Visibility
    /// Count = 0 -> Visible (hiển thị empty message)
    /// Count > 0 -> Collapsed (ẩn empty message)
    /// </summary>
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                // Nếu count = 0, hiển thị message "giỏ hàng trống"
                // Nếu count > 0, ẩn message
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
