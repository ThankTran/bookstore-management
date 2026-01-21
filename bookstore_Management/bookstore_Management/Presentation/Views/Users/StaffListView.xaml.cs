using bookstore_Management.Core.Enums;
using bookstore_Management.Models;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace bookstore_Management.Presentation.Views.Users
{
    /// <summary>
    /// Interaction logic for StaffListView.xaml
    /// </summary>
    public partial class StaffListView : UserControl
    {
        private StaffViewModel _viewModel;
        public StaffListView(StaffViewModel staffViewModel) 
        {            
            InitializeComponent();
            this.DataContext = staffViewModel;
        }

        private void dgStaff_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
