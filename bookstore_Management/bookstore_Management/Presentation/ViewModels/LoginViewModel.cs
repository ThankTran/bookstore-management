using bookstore_Management.Services;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Views;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            set { _username = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;

            LoginCommand = new RelayCommand<object>(LoginProcessAsync);
            CloseCommand = new AsyncRelayCommand<object>(async p =>
            {
                await Task.Run(() => Application.Current.Shutdown());
            });
        }

        private void LoginProcessAsync(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var loginResult = _userService.LoginAsync(Username, password);
                if (!loginResult.IsSuccess || loginResult.Data == false)
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!",
                                    "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);

                    if (passwordBox != null) passwordBox.Password = "";
                    return;
                }

                var userResult = _userService.GetByUsername(Username);
                if (!userResult.IsSuccess || userResult.Data == null)
                {
                    MessageBox.Show("Lỗi hệ thống: Không tải được thông tin người dùng.",
                                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var currentUser = userResult.Data.FirstOrDefault();
                SessionService.Instance.StartSession(currentUser);

                string displayName = !string.IsNullOrEmpty(currentUser.UserName) ? currentUser.UserName : currentUser.UserName;
                MessageBox.Show($"Xin chào {displayName}!\nQuyền truy cập: {currentUser.Role}",
                                "Đăng nhập thành công");

                var mainWindow = new MainWindow();
                mainWindow.Show();

                var currentWindow = Window.GetWindow(passwordBox);
                currentWindow?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
