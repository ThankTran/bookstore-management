using bookstore_Management.Models;
using bookstore_Management.Services;
using bookstore_Management.Services.Interfaces; // Chứa IUserService
using bookstore_Management.Views; // Chứa MainWindow
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls; // Quan trọng: Để dùng PasswordBox
using System.Windows.Input;

namespace bookstore_Management.Presentation.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand LoginCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;

            LoginCommand = new RelayCommand<object>((p) => LoginProcess(p));

            CloseCommand = new RelayCommand<object>((p) =>
            {
                Application.Current.Shutdown();
            });
        }

        private void LoginProcess(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!");
                return;
            }

            var loginResult = _userService.Login(Username, password);
            if (!loginResult.IsSuccess || loginResult.Data == false)
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!");
                if (passwordBox != null) passwordBox.Password = "";
                return;
            }

            var userResult = _userService.GetByUsername(Username);
            if (!userResult.IsSuccess)
            {
                MessageBox.Show(userResult.ErrorMessage);
                return;
            }

            var currentUser = userResult.Data;

            SessionModel.UserId = currentUser.AccountId;
            SessionModel.Username = currentUser.UserName;
            SessionModel.Role = currentUser.Role;

            var mainWindow = new MainWindow();
            mainWindow.Show();

            Window.GetWindow(passwordBox)?.Close();
        }


    }
    
}