using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace bookstore_Management.Presentation.Converters
{
    /// <summary>
    /// Converter chuyển Count (int) sang Visibility
    ///
    /// Cách dùng:
    /// - Không truyền parameter:
    ///     Count == 0  -> Visible
    ///     Count > 0   -> Collapsed
    ///
    /// - ConverterParameter="Inverse":
    ///     Count > 0   -> Visible
    ///     Count == 0  -> Collapsed
    /// </summary>
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Kiểm tra kiểu dữ liệu (C# < 8)
            if (value == null || !(value is int))
                return Visibility.Collapsed;

            int count = (int)value;

            bool inverse = parameter != null && parameter.ToString() == "Inverse";

            if (!inverse)
            {
                // Mặc định: hiện khi không có item
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            // Inverse: hiện khi có item
            return count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
