using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace bookstore_Management.Presentation.Converters
{
    

    /// <summary>
    /// Converter ngược lại - hiển thị khi có items
    /// Count = 0 -> Collapsed
    /// Count > 0 -> Visible
    /// </summary>
    public class InverseCountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                // Nếu count > 0, hiển thị danh sách items
                // Nếu count = 0, ẩn danh sách
                return count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    

    /// <summary>
    /// Converter ngược Boolean thành Visibility
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Collapsed;
            }

            return true;
        }
    }
}