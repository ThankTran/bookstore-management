using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Services.Interfaces;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using bookstore_Management.Data.Context;
using bookstore_Management.Services.Implementations;

namespace bookstore_Management.Presentation.Views.Users
{
    /// <summary>
    /// Interaction logic for AccountListView.xaml
    /// </summary>
    public partial class AccountListView : UserControl
    {
        private UserViewModel _viewModel;
        public AccountListView()
        {
            InitializeComponent();
            var context = new BookstoreDbContext();
            IUserService userService;
            userService = new UserService(
                new Data.Repositories.Implementations.UserRepository(context),
                new Data.Repositories.Implementations.StaffRepository(context)
            );
            _viewModel = new UserViewModel(userService);
            this.DataContext = _viewModel;

        }

        private void dgAccounts_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }


    }
}
