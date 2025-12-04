using System.Windows;

namespace bookstore_Management.Presentation.Views.Dialogs.Customers
{
    public partial class DeleteCustomer : Window
    {
        public bool IsConfirmed { get; private set; }

        public DeleteCustomer()
        {
            InitializeComponent();
        }

        // Xác nhận xóa
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            DialogResult = true;
            Close();
        }

        // Hủy
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            DialogResult = false;
            Close();
        }
    }
}
