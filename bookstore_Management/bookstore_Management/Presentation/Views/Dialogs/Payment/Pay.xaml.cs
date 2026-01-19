using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using bookstore_Management.DTOs.Order.Responses;

namespace bookstore_Management.Presentation.Views.Dialogs.Payment
{
    /// <summary>
    /// Dialog hiển thị và in hóa đơn thanh toán
    /// </summary>
    public partial class Pay : Window
    {
        private readonly OrderResponseDto _orderData;

        public Pay(OrderResponseDto orderData)
        {
            InitializeComponent();

            _orderData = orderData ?? throw new ArgumentNullException(nameof(orderData));
        }

        #region Window Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBillData();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion

        #region Load Data

        private void LoadBillData()
        {
            try
            {
                // Basic Info
                tbBillId.Text = _orderData.OrderId;
                tbStaffName.Text = _orderData.StaffName ?? "N/A";
                tbCustomerName.Text = _orderData.CustomerName ?? "Khách vãng lai";
                tbDate.Text = _orderData.CreatedDate.ToString("dd/MM/yyyy");
                tbTime.Text = _orderData.CreatedDate.ToString("HH:mm");
                tbPaymentMethod.Text = MapPaymentMethodToDisplay(_orderData.PaymentMethod);

                // Items
                var items = _orderData.OrderDetails?
                    .Select((detail, index) => new BillItemDisplay
                    {
                        STT = index + 1,
                        BookName = detail.BookName,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.SalePrice,
                        Subtotal = detail.Subtotal
                    })
                    .ToList() ?? new List<BillItemDisplay>();

                icItems.ItemsSource = items;

                // Summary
                var subtotal = items.Sum(x => x.Subtotal);
                var discountVnd = subtotal * _orderData.Discount; // Discount is rate (0..1)
                var total = _orderData.TotalPrice;

                tbSubtotal.Text = $"{subtotal:N0} ₫";
                tbDiscount.Text = $"{discountVnd:N0} ₫";
                tbTotal.Text = $"{total:N0} ₫";

                // Total in words
                tbTotalInWords.Text = $"(Bằng chữ: {NumberToWords((long)total)})";

                // Notes (optional)
                if (!string.IsNullOrWhiteSpace(_orderData.Notes))
                {
                    tbNotes.Text = $"Ghi chú: {_orderData.Notes}";
                    tbNotes.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi tải dữ liệu hóa đơn: {ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Print Logic

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create print dialog
                var printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    // Get the printable area
                    var printArea = printableArea;

                    // Store original size
                    var originalWidth = printArea.ActualWidth;
                    var originalHeight = printArea.ActualHeight;

                    // Calculate scaling
                    var pageWidth = printDialog.PrintableAreaWidth;
                    var pageHeight = printDialog.PrintableAreaHeight;

                    // Scale to fit page width
                    var scale = Math.Min(pageWidth / originalWidth, pageHeight / originalHeight);

                    printArea.LayoutTransform = new ScaleTransform(scale, scale);

                    // Arrange and measure
                    var size = new Size(pageWidth, pageHeight);
                    printArea.Measure(size);
                    printArea.Arrange(new Rect(new Point(0, 0), printArea.DesiredSize));

                    // Print
                    printDialog.PrintVisual(printArea, $"Hóa đơn {_orderData.OrderId}");

                    // Reset transform
                    printArea.LayoutTransform = Transform.Identity;
                    printArea.InvalidateVisual();

                    MessageBox.Show(
                        "In hóa đơn thành công!",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi in: {ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Helper Methods

        private string MapPaymentMethodToDisplay(Core.Enums.PaymentType paymentType)
        {
            switch (paymentType)
            {
                case Core.Enums.PaymentType.Cash:
                    return "💵 Tiền mặt";

                case Core.Enums.PaymentType.Card:
                    return "💳 Quẹt thẻ";

                case Core.Enums.PaymentType.BankTransfer:
                    return "📱 Chuyển khoản";

                case Core.Enums.PaymentType.DebitCard:
                    return "👜 Thẻ tín dụng";

                default:
                    return "Khác";
            }
        }


        /// <summary>
        /// Convert number to Vietnamese words
        /// </summary>
        private string NumberToWords(long number)
        {
            if (number == 0) return "Không đồng";

            string[] ones = { "", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] tens = { "", "mười", "hai mươi", "ba mươi", "bốn mươi", "năm mươi",
                             "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };

            if (number < 10)
                return CapitalizeFirst(ones[number] + " đồng");

            if (number < 20)
            {
                var unit = number % 10;
                return CapitalizeFirst($"mười {ones[unit]} đồng".Trim());
            }

            if (number < 100)
            {
                var ten = number / 10;
                var unit = number % 10;
                var result = $"{tens[ten]} {ones[unit]}".Trim();
                return CapitalizeFirst(result + " đồng");
            }

            if (number < 1000)
            {
                var hundred = number / 100;
                var remainder = number % 100;
                var result = $"{ones[hundred]} trăm";

                if (remainder > 0)
                {
                    if (remainder < 10)
                        result += $" lẻ {ones[remainder]}";
                    else if (remainder < 20)
                        result += $" mười {ones[remainder % 10]}";
                    else
                    {
                        var ten = remainder / 10;
                        var unit = remainder % 10;
                        result += $" {tens[ten]} {ones[unit]}".Trim();
                    }
                }

                return CapitalizeFirst(result + " đồng");
            }

            // For larger numbers, simplified version
            if (number < 1000000)
            {
                var thousand = number / 1000;
                var remainder = number % 1000;
                var result = NumberToWords(thousand).Replace(" đồng", "") + " nghìn";

                if (remainder > 0)
                    result += " " + NumberToWords(remainder).Replace(" đồng", "");

                return CapitalizeFirst(result + " đồng");
            }

            if (number < 1000000000)
            {
                var million = number / 1000000;
                var remainder = number % 1000000;
                var result = NumberToWords(million).Replace(" đồng", "") + " triệu";

                if (remainder > 0)
                    result += " " + NumberToWords(remainder).Replace(" đồng", "");

                return CapitalizeFirst(result + " đồng");
            }

            // Billion
            var billion = number / 1000000000;
            var rest = number % 1000000000;
            var finalResult = NumberToWords(billion).Replace(" đồng", "") + " tỷ";

            if (rest > 0)
                finalResult += " " + NumberToWords(rest).Replace(" đồng", "");

            return CapitalizeFirst(finalResult + " đồng");
        }

        private string CapitalizeFirst(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Trim();
            return char.ToUpper(text[0]) + text.Substring(1);
        }

        #endregion

        #region Display Classes

        private class BillItemDisplay
        {
            public int STT { get; set; }
            public string BookName { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Subtotal { get; set; }
        }

        #endregion
    }
}