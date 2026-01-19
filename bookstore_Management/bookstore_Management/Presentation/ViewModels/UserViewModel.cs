using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class UserViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                SearchUserCommand.Execute(null);
            }
        }

        public Array UserRoles =>Enum.GetValues(typeof(UserRole));
        private UserRole _role;
        public UserRole Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged();
            }
        }

        public string RoleDisplay
        {
            get
            {
                switch (Role)
                {
                    case UserRole.Administrator:
                        return "Quản trị";
                    case UserRole.SalesManager:
                        return "Quản lý bán hàng";
                    case UserRole.InventoryManager:
                        return "Quản lý kho";
                    default:
                        return "Không xác định";
                }
            }
        }


        #region Khai báo command
        //khai báo command cho thao tác thêm, xóa, sửa account
        public ICommand AddUserCommand { get; set; }
        public ICommand RemoveUserCommand { get; set; }
        public ICommand EditUserCommand { get; set; }
        public ICommand ChangePassword { get; set; }

        //command cho thao tác tìm kiếm - load lại
        public ICommand SearchUserCommand { get; set; }
        public ICommand LoadData { get; set; }

        //command cho in / xuất excel
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        #endregion

        private void LoadUsersFromDatabase()
        {
            var result = _userService.GetAll();
            if (!result.IsSuccess)
            {
                // Xử lý lỗi nếu cần
                return;
            }
            if (result.Data == null) return;

            var users = result.Data.Select(user => new User
            {
                Username = user.UserName,
                PasswordHash = user.Password,
                StaffId = user.StaffId,
                UserRole = user.Role,
                CreatedDate = user.CreateDate,
            });
            Users = new ObservableCollection<User>(users);
        }

        public UserViewModel(IUserService userService)
        {

            _userService = userService;

            Users = new ObservableCollection<User>();

            LoadUsersFromDatabase();

            #region AddCommand
            AddUserCommand = new RelayCommand<object>((p) =>
            {
                var dialog = new Views.Dialogs.Accounts.AddAccountDialog();
                if (dialog.ShowDialog() == true)
                {
                    // Call service to add to database
                    var newUserDto = new DTOs.User.Requests.CreateUserRequestDto
                    {
                        Username = dialog.Username,
                        Password = dialog.Password,
                        StaffId = dialog.SelectedStaffID
                    };
                    var result = _userService.CreateUser(newUserDto);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Lỗi khi thêm account: " + result.ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    // Reload from database
                    LoadUsersFromDatabase();
                }
            });
            #endregion
            #region RemoveCommand
            RemoveUserCommand = new RelayCommand<object>((p) =>
            {
                var user = p as User;
                if (user == null)
                {
                    MessageBox.Show("Vui lòng chọn account để xóa");
                    return;
                }

                bool confirmed = Views.Dialogs.Share.Delete.ShowForAccount(
                    user.Username
                );

                if (!confirmed) return;

                var result = _userService.Deactivate(user.Username);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi xóa: " + result.ErrorMessage,
                                    "Lỗi",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }
                LoadUsersFromDatabase();
            });
            #endregion
            #region EditCommand
            EditUserCommand = new RelayCommand<object>((p) =>
            {
                var dialog = new Views.Dialogs.Accounts.UpdateAccount();
                var user = p as User;
                if (user == null)
                {
                    MessageBox.Show("Vui lòng chọn để chỉnh sửa");
                    return;
                }

                dialog.Password = user.PasswordHash;

                if (dialog.ShowDialog() == true)
                {
                    var updateDto = new DTOs.User.Requests.ChangePasswordRequestDto
                    { 
                        NewPassword = dialog.Password
                    };

                    var result = _userService.ChangePassword(user.Username, updateDto);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Lỗi khi cập nhật / chỉnh sửa sách");
                        return;
                    }

                    LoadUsersFromDatabase();
                }
            });
            #endregion
            #region SearchCommand
            SearchUserCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(SearchKeyword))
                {
                    LoadUsersFromDatabase();//k nhập gì thì hiện lại list
                    return;
                }

                var result = _userService.SearchByUsername(SearchKeyword);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi tìm ");
                    return;
                }
                Users.Clear();
                //foreach (var u in result.Data)
                //{
                //    Books.Add(new Book
                //    {
                //        BookId = b.BookId,
                //        Name = b.Name,
                //        Author = b.Author,
                //        Category = b.Category,
                //        SalePrice = b.SalePrice,
                //        Publisher = new Publisher
                //        {
                //            Name = b.PublisherName
                //        },
                //    });
                //}
            });
            #endregion
            #region LoadDataCommand
            LoadData = new RelayCommand<object>((p) =>
            {
                SearchKeyword = string.Empty;
                LoadUsersFromDatabase();
            });
            #endregion

            ChangePassword = new RelayCommand<object>((p) =>
            {
                //var dialog = new Views.Dialogs.Accounts.UpdateAccountDialog();
                //if (dialog.ShowDialog() == true)
                //{
                //    var changePasswordDto = new DTOs.User.Requests.ChangePasswordRequestDto
                //    {
                //        Username = dialog.Username,
                //        OldPassword = dialog.OldPassword,
                //        NewPassword = dialog.NewPassword
                //    };
                //    var result = _userService.ChangePassword(changePasswordDto);
                //    if (!result.IsSuccess)
                //    {
                //        MessageBox.Show("Lỗi khi đổi mật khẩu: " + result.ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                //        return;
                //    }
                //    MessageBox.Show("Đổi mật khẩu thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                //}
            });

        }
    }
}
