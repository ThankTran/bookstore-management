using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Presentation.ViewModels;

namespace bookstore_Management.Presentation.Views.Customers
{
    public partial class CustomerListView : UserControl
    {
        public event EventHandler<Customer> CustomerSelected;

        public CustomerListView()
        {
            InitializeComponent();
            var context = new BookstoreDbContext();
            var unitOfWork = new UnitOfWork(context);
            var service = new CustomerService(unitOfWork);

            DataContext = new CustomerViewModel(service);
            
            Loaded += CustomerListView_Loaded;
        }

        private void dgCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer selectedCustomer)
            {
                CustomerSelected?.Invoke(this, selectedCustomer);
            }
        }

        private void dgCustomers_LoadingRow(object sender, DataGridRowEventArgs e)
        {          
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();        
        }
        private async void CustomerListView_Loaded(object sender, RoutedEventArgs e)
        {
            await ((CustomerViewModel)DataContext).LoadCusFromDatabase();
        }
    }
}
