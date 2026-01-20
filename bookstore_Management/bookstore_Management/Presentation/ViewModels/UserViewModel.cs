using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Models;
using bookstore_Management.Presentation.Views.Dialogs.Accounts;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class UserViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly StaffRepository _staffRepository;

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set { _users = value; OnPropertyChanged(); }
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                SearchUserCommand?.Execute(null);
            }
        }

        public Array UserRoles => Enum.GetValues(typeof(UserRole));

        public ICommand AddUserCommand { get; set; }
        public ICommand RemoveUserCommand { get; set; }
        public ICommand EditUserCommand { get; set; }
        public ICommand ChangePassword { get; set; }
        public ICommand SearchUserCommand { get; set; }
        public ICommand LoadData { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        public UserViewModel(IUserService userService)
        {
            var context = new BookstoreDbContext();
            _staffRepository = new StaffRepository(context);
            _userService = userService;

            Users = new ObservableCollection<User>();

            AddUserCommand = new AsyncRelayCommand(AddCommandAsync);
            RemoveUserCommand = new AsyncRelayCommand(RemoveCommandAsync);
            EditUserCommand = new AsyncRelayCommand(EditCommandAsync);
            SearchUserCommand = new AsyncRelayCommand(SearchCommandAsync);
            LoadData = new AsyncRelayCommand(LoadDataCommandAsync);
            PrintCommand = new AsyncRelayCommand(PrintCommandAsync);
            ExportCommand = new AsyncRelayCommand(ExportCommandAsync);
        }

        private async Task LoadUsersFromDatabase()
        {
            var result = await _userService.GetAllAsync();
            if (!result.IsSuccess || result.Data == null) return;

            Users = new ObservableCollection<User>(
                result.Data.Select(u => new User
                {
                    UserId = u.AccountId,
                    StaffId = u.StaffId,
                    Username = u.UserName,
                    UserRole = u.Role,
                    CreatedDate = u.CreateDate
                })
            );
        }

        private async Task AddCommandAsync()
        {
            var dialog = new AddAccountDialog();
            var staffs =  await _staffRepository.GetAllAsync();
            var staffIds = staffs.Select(x => x.Id).ToList();
            dialog.LoadStaffId(staffIds);

            if (dialog.ShowDialog() == true)
            {
                var dto = new DTOs.User.Requests.CreateUserRequestDto
                {
                    Username = dialog.Username,
                    Password = dialog.Password,
                    StaffId = dialog.SelectedStaffID
                };
                var result = await _userService.CreateUserAsync(dto);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi thêm account: " + result.ErrorMessage);
                    return;
                }
                await LoadUsersFromDatabase();
            }
        }

        private async Task RemoveCommandAsync()
        {
            var user = SelectedUser;
            if (user == null)
            {
                MessageBox.Show("Vui lòng chọn account để xóa");
                return;
            }

            bool confirmed = Views.Dialogs.Share.Delete.ShowForAccount(user.Username);
            if (!confirmed) return;

            var result = await _userService.DeactivateAsync(user.Username);
            if (!result.IsSuccess)
            {
                MessageBox.Show("Lỗi khi xóa: " + result.ErrorMessage);
                return;
            }
            await LoadUsersFromDatabase();
        }

        private async Task EditCommandAsync()
        {
            try
            {
                var user = SelectedUser;
                if (user == null)
                {
                    MessageBox.Show("Vui lòng chọn account để chỉnh sửa");
                    return;
                }

                var dialog = new UpdateAccount();
                var staffs = await _staffRepository.GetAllAsync();
                var staffIds = staffs.Select(s => s.Id).ToList();
                dialog.LoadStaffIds(staffIds);

                dialog.Account = user.StaffId ?? "";

                if (dialog.ShowDialog() == true)
                {
                    string newPassword = dialog.Password;
                    if (string.IsNullOrWhiteSpace(newPassword))
                    {
                        MessageBox.Show("Mật khẩu mới không được để trống.");
                        return;
                    }

                    var dto = new DTOs.User.Requests.ChangePasswordRequestDto
                    {
                        NewPassword = newPassword
                    };

                    var result = await _userService.ChangePasswordAsync(user.StaffId, dto);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Service báo lỗi: " + result.ErrorMessage);
                        return;
                    }

                    MessageBox.Show("Đổi mật khẩu thành công!");
                    await LoadUsersFromDatabase();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Crash ứng dụng: {ex.Message}");
            }
        }

        private async Task SearchCommandAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                await LoadUsersFromDatabase();
                return;
            }

            var result = _userService.GetByUsername(SearchKeyword);
            if (!result.IsSuccess || result.Data == null)
            {
                Users.Clear();
                return;
            }

            Users = new ObservableCollection<User>(
                result.Data.Select(u => new User
                {
                    UserId = u.AccountId,
                    StaffId = u.StaffId,
                    Username = u.UserName,
                    UserRole = u.Role,
                    CreatedDate = u.CreateDate
                })
            );
        }

        private async Task LoadDataCommandAsync()
        {
            SearchKeyword = string.Empty;
            await LoadUsersFromDatabase();
        }

        private async Task PrintCommandAsync()
        {
            var data = Users;
            var dialog = new PrintAccount(data);
            dialog.ShowDialog();
        }

        private async Task ExportCommandAsync()
        {
            // TODO: xuất Excel
        }
    }
}
