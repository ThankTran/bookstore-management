using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Presentation.Views.Dialogs.Customers;
using bookstore_Management.Presentation.Views.Dialogs.Payment;
using System.Windows;
using System.Windows.Controls;

namespace bookstore_Management.Presentation.Views.Payment
{
    public partial class PaymentView : UserControl
    {
        public PaymentView(PaymentViewModel paymentViewModel)
        {
            InitializeComponent();

            var vm = paymentViewModel;
            DataContext = vm;

            // subscribe event 1 lần duy nhất
            vm.RequestOpenAddCustomerDialog += OnOpenAddCustomerDialog;
            vm.RequestOpenPayDialog += () =>
            {
                var dialog = new Pay(vm)
                {
                    Owner = Window.GetWindow(this)
                };

                if (dialog.ShowDialog() == true && dialog.IsConfirmed)
                {
                    vm.ConfirmCheckout(); 
                }
            };

        }


        private void OnOpenAddCustomerDialog()
        {
            var dialog = new AddCustomer
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true)
            {
                if (DataContext is PaymentViewModel vm)
                {
                    vm.ReloadCustomersAfterAdd();
                }
            }
        }
    }
}
