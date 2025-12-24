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
        //public string Category
        //{
        //    get { return tbCategory.Text; }
        //    set { tbCategory.Text = value; }
        //}
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
            this.DialogResult = true;
            this.Close();
        }
    }
}
