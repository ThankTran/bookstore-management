using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Presentation.Views.Dialogs.Books
{
    /// <summary>
    /// Interaction logic for InputBooksDialog.xaml
    /// </summary>
    public partial class InputBooksDialog : Window
    {
        public InputBooksDialog()
        {
            InitializeComponent();
            cbCategory.ItemsSource = Enum.GetValues(typeof(BookCategory));//lấy item từ enum BookCategory
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        //lấy data từ textbox
        public string BookID
        {
            get { return tbBookID.Text; }
            set { tbBookID.Text = value; }
        }
        public string BookName
        {
            get { return tbBookName.Text; }
            set { tbBookName.Text = value; }
        }
        public string Author
        {
            get { return tbAuthor.Text; }
            set { tbAuthor.Text = value; }
        }
        public BookCategory Category
        {
            get { return (BookCategory)cbCategory.SelectedItem; }
            set { cbCategory.SelectedItem = value; }
        }
        public string Publisher
        {
            get { return tbPublisher.Text; }
            set { tbPublisher.Text = value; }
        }
        public decimal SalePrice => decimal.TryParse(tbSalePrice.Text, out var val) ? val : 0;
        public decimal ImportPrice => decimal.TryParse(tbImportPrice.Text, out var val) ? val : 0;

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // You can add validation logic here before closing the dialog
            if (string.IsNullOrEmpty(tbBookID.Text) || string.IsNullOrEmpty(tbBookName.Text) || string.IsNullOrEmpty(tbAuthor.Text) || string.IsNullOrEmpty(tbPublisher.Text)) { return; }

            this.DialogResult = true;
            this.Close();
        }
    }
}