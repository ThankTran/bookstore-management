using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace bookstore_Management.Presentation.Views.Users
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            IUserService userService;
            var context = new BookstoreDbContext();

            userService = new UserService(
                new UserRepository(context),
                new StaffRepository(context)
            );
             
            DataContext = new LoginViewModel(userService);

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            this.Close();
        }

        private void btnCancelLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
