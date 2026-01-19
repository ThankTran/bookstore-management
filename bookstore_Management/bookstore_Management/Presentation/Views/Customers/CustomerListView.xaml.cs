using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.Presentation.ViewModels;

namespace bookstore_Management.Views.Customers
{
    public partial class CustomerListView : UserControl
    {
        public ObservableCollection<Customer> Customers { get; set; }
        public event EventHandler<Customer> CustomerSelected;

        public CustomerListView()
        {
            InitializeComponent();
         
            //var context = new BookstoreDbContext();
            //_customerService = new CustomerService(
            //    new CustomerRepository(context),
            //    new OrderRepository(context)
            //);
            var _viewModel = new CustomerViewModel();
            this.DataContext = _viewModel;
            //LoadSampleData();
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
    }
}
