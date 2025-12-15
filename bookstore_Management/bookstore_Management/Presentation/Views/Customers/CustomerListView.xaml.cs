using bookstore_Management.Presentation.Views.Dialogs.Customers;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace bookstore_Management.Views.Customers
{
    public partial class CustomerListView : UserControl
    {
        public ObservableCollection<Customer> Customers { get; set; }
        public event EventHandler<Customer> CustomerSelected;

        public CustomerListView()
        {
            InitializeComponent();
            DataContext = this;
            LoadSampleData();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var owner = Window.GetWindow(this);
            var dialog = new AddCustomer();

            dialog.WindowState = WindowState.Normal;
            dialog.Width = 456;
            dialog.Height = 300;

            if (owner != null)
            {
                dialog.Owner = owner;
                UpdateLayout();

                var pos = PointToScreen(new Point(0, 0));

                double left = pos.X + (ActualWidth - dialog.Width) / 2;
                double top = pos.Y + (ActualHeight - dialog.Height) / 2;

                dialog.Left = Math.Round(left);
                dialog.Top = Math.Round(top);
            }

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                var newCustomer = new Customer
                {
                    STT = Customers.Count + 1,
                    MaKH = $"KH{100 + Customers.Count}",
                    TenKH = dialog.CustomerName,
                    SDT = dialog.Phone,
                    HangThanhVien = "Đồng",
                    DoanhThu = 0
                };

                Customers.Add(newCustomer);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Customer customer)
            {
                var owner = Window.GetWindow(this);
                var editDialog = new EditCustomer(customer);

                // Ép trạng thái & kích thước đúng như design
                editDialog.WindowState = WindowState.Normal;
                editDialog.Width = 456;
                editDialog.Height = 316;

                if (owner != null)
                {
                    editDialog.Owner = owner;
                    UpdateLayout();

                    var pos = PointToScreen(new Point(0, 0));

                    double left = pos.X + (ActualWidth - editDialog.Width) / 2;
                    double top = pos.Y + (ActualHeight - editDialog.Height) / 2;

                    editDialog.Left = Math.Round(left);
                    editDialog.Top = Math.Round(top);
                }

                bool? result = editDialog.ShowDialog();

                if (result == true)
                {
                    dgCustomers.Items.Refresh();

                    MessageBox.Show("Đã cập nhật thông tin khách hàng!",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }



        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Customer selectedCustomer)
            {
                var owner = Window.GetWindow(this);
                var dialog = new DeleteCustomer();

                if (owner != null)
                {
                    dialog.Owner = owner;
                    UpdateLayout();

                    var pos = PointToScreen(new Point(0, 0));

                    double left = pos.X + (ActualWidth - dialog.Width) / 2;
                    double top = pos.Y + (ActualHeight - dialog.Height) / 2;

                    dialog.Left = Math.Round(left);
                    dialog.Top = Math.Round(top);
                }

                bool? result = dialog.ShowDialog();

                if (result == true && dialog.IsConfirmed)
                {
                    Customers.Remove(selectedCustomer);
                    ReindexCustomers();
                }
            }
        }

        private void ReindexCustomers()
        {
            int index = 1;
            foreach (var c in Customers)
                c.STT = index++;

            dgCustomers.Items.Refresh();
        }

        private void dgCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer selectedCustomer)
            {
                CustomerSelected?.Invoke(this, selectedCustomer);
            }
        }

        private void LoadSampleData()
        {
            Customers = new ObservableCollection<Customer>
            {
                new Customer { STT = 1, MaKH = "KH100", TenKH = "Trần Thị Hồng Thanh", HangThanhVien = "Kim Cương", SDT = "0593793875", DoanhThu = 2760000000 },
                new Customer { STT = 2, MaKH = "KH101", TenKH = "Nguyễn Ái My", HangThanhVien = "Bạch Kim", SDT = "0861278318", DoanhThu = 500000000 },
                new Customer { STT = 3, MaKH = "KH102", TenKH = "Phạm Hoàng Gia Hiển", HangThanhVien = "Vàng", SDT = "0826437621", DoanhThu = 1500000000 },
                new Customer { STT = 4, MaKH = "KH103", TenKH = "Phạm Nguyên Gia Huy", HangThanhVien = "Kim Cương", SDT = "0782648645", DoanhThu = 700000000 },
                new Customer { STT = 5, MaKH = "KH104", TenKH = "Nguyễn Khánh Vi", HangThanhVien = "Đồng", SDT = "0173288246", DoanhThu = 300000000 },
                new Customer { STT = 6, MaKH = "KH105", TenKH = "Nguyễn Quang Nghĩa", HangThanhVien = "Bạc", SDT = "0861278318", DoanhThu = 2760000000 },
                new Customer { STT = 7, MaKH = "KH106", TenKH = "Võ Nguyên Khoa", HangThanhVien = "Bạc", SDT = "0826437621", DoanhThu = 500000000 },
                new Customer { STT = 8, MaKH = "KH107", TenKH = "Nguyễn Vũ Phúc", HangThanhVien = "Đồng", SDT = "0782648645", DoanhThu = 1500000000 },
                new Customer { STT = 9, MaKH = "KH108", TenKH = "Nguyễn Ngọc Hân", HangThanhVien = "Bạch Kim", SDT = "0173288246", DoanhThu = 700000000 },
                new Customer { STT = 10, MaKH = "KH109", TenKH = "Lê Minh Tuấn", HangThanhVien = "Vàng", SDT = "0912345678", DoanhThu = 1200000000 },
                new Customer { STT = 11, MaKH = "KH110", TenKH = "Trần Văn Nam", HangThanhVien = "Bạc", SDT = "0923456789", DoanhThu = 800000000 },
                new Customer { STT = 12, MaKH = "KH111", TenKH = "Nguyễn Thị Lan", HangThanhVien = "Đồng", SDT = "0934567890", DoanhThu = 450000000 },
            };

            dgCustomers.ItemsSource = Customers;
        }
    }

    public class Customer
    {
        public int STT { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string HangThanhVien { get; set; }
        public string SDT { get; set; }
        public long DoanhThu { get; set; }
    }
}
