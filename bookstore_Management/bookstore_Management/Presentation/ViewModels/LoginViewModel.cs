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

        internal LoginViewModel(IUserService userService)
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
            MessageBox.Show("LoginCommand đã được gọi");

            // 1. Lấy mật khẩu từ PasswordBox
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            // 2. Kiểm tra dữ liệu nhập
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 3. Gọi Service kiểm tra tài khoản
                var loginResult = _userService.Login(Username, password);

                if (!loginResult.IsSuccess || loginResult.Data == false)
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!",
                                    "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);

                    if (passwordBox != null) passwordBox.Password = "";
                    return;
                }

                // 4. Lấy thông tin chi tiết User để lưu vào Session
                var userResult = _userService.SearchByUsername(Username);

                if (!userResult.IsSuccess || userResult.Data == null || !userResult.Data.Any())
                {
                    MessageBox.Show("Lỗi hệ thống: Không tải được thông tin người dùng.",
                                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var currentUser = userResult.Data.FirstOrDefault();

                // 5. --- LƯU SESSION ---
                SessionService.Instance.StartSession(currentUser);

                // 6. Hiển thị thông báo chào mừng
                string displayName = !string.IsNullOrEmpty(currentUser.UserName) ? currentUser.UserName : currentUser.UserName;
                MessageBox.Show($"Xin chào {displayName}!\nQuyền truy cập: {currentUser.Role}",
                                "Đăng nhập thành công");

                // 7. Mở màn hình chính
                var mainWindow = new MainWindow();
                mainWindow.Show();

                // 8. Đóng màn hình Login
                // Tìm cửa sổ chứa PasswordBox này để đóng
                var currentWindow = Window.GetWindow(passwordBox);
                if (currentWindow != null)
                {
                    currentWindow.Close();
                }
                else
                {
                    // Fallback: Đóng cửa sổ đang active nếu không tìm thấy
                    Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}