using System.Windows.Controls;
using bookstore_Management.ViewModels;

namespace bookstore_Management.Views.Orders
{
    /// <summary>
    /// Interaction logic for PaymentView.xaml
    /// </summary>
    public partial class PaymentView : UserControl
    {
        public PaymentView()
        {
            InitializeComponent();
            //DataContext = new PaymentViewModel();
        }
    }
}