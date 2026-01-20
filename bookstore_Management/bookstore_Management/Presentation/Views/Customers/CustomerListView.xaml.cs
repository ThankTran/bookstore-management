using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Presentation.ViewModels;

namespace bookstore_Management.Views.Customers
{
    public partial class CustomerListView : UserControl
    {
        public ObservableCollection<Customer> Customers { get; set; }
        public event EventHandler<Customer> CustomerSelected;

        public CustomerListView(CustomerViewModel customerViewModel)
        {
            InitializeComponent();
            DataContext = customerViewModel;
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
