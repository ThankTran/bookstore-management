using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;
using bookstore_Management.Presentation.Views.Dialogs.Accounts;
using bookstore_Management.Presentation.Views.Dialogs.Staffs;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using bookstore_Management.DTOs.User.Response;

namespace bookstore_Management.Presentation.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IStaffRepository _staffRepository;

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
            get => _searchKeyword;
            set
            {
                if (_searchKeyword != value)
                {
                    _searchKeyword = value;
                    OnPropertyChanged();
                    SearchUser();
                }
            }
        }


        public Array UserRoles =>Enum.GetValues(typeof(UserRole));
        //private UserRole _role;
        //public UserRole Role
        //{
        //    get => _role;
        //    set
        //    {
        //        _role = value;
        //        OnPropertyChanged();
        //    }
        //}

        


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

        private void SearchUser()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                LoadUsersFromDatabase();
                return;
            }

            var result = _userService.SearchByUsername(SearchKeyword);

            if (!result.IsSuccess || result.Data == null)
            {
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


        public UserViewModel(IUserService userService, IStaffRepository  staffRepository)
        {
            _userService = userService;
            _staffRepository = staffRepository;
            
            Users = new ObservableCollection<User>();

            LoadUsersFromDatabase();

            #region AddCommand
            AddUserCommand = new RelayCommand<object>((p) =>
            {
                if (SessionModel.Role == UserRole.SalesManager)
                {
                    MessageBox.Show("Bạn không có quyền này");
                    return;
                }
                var dialog = new Views.Dialogs.Accounts.AddAccountDialog();

                var staffs = _staffRepository.GetAll();
                var staffIds = staffs.Select(x =>x.Id).ToList();
                dialog.LoadStaffId(staffIds);
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
                if (SessionModel.Role == UserRole.SalesManager)
                {
                    MessageBox.Show("Bạn không có quyền này");
                    return;
                }
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
            //EditUserCommand = new RelayCommand<object>((p) =>
            //{
            //    var dialog = new Views.Dialogs.Accounts.UpdateAccount();
            //    var user = p as User;


            //    var staffs = _staffRepository.GetAll();
            //    var staffIds = staffs.Select(s => s.Id).ToList();
            //    if (user == null)
            //    {
            //        MessageBox.Show("Vui lòng chọn để chỉnh sửa");
            //        return;
            //    }

            //    dialog.LoadStaffIds(staffIds);
            //    dialog.Account = user.StaffId;

            //    if (dialog.ShowDialog() == true)
            //    {
            //        string newPassword = dialog.Password;

            //        // 1. Kiểm tra: Nếu người dùng không nhập gì (hoặc chỉ nhập khoảng trắng)
            //        if (string.IsNullOrWhiteSpace(newPassword))
            //        {
            //            MessageBox.Show("Bạn chưa nhập mật khẩu mới. Không có thay đổi nào được lưu.");
            //            return; // Thoát luôn, không gọi Service
            //        }

            //        // 2. Nếu có nhập, mới tạo DTO và gọi Service
            //        var updateDto = new DTOs.User.Requests.ChangePasswordRequestDto
            //        {
            //            NewPassword = newPassword,
            //        };

            //        var result = _userService.ChangePassword(user.StaffId, updateDto);

            //        if (!result.IsSuccess)
            //        {
            //            // Hiển thị lỗi chi tiết từ Server
            //            MessageBox.Show($"Lỗi cập nhật: {result.ErrorMessage}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            //            return;
            //        }

            //        MessageBox.Show("Đổi mật khẩu thành công!");
            //        LoadUsersFromDatabase();
            //    }
            //});

            #region EditCommand
            EditUserCommand = new RelayCommand<object>((p) =>
            {
                if (SessionModel.Role == UserRole.SalesManager)
                {
                    MessageBox.Show("Bạn không có quyền này");
                    return;
                }
                try
                {
                    // 1. Kiểm tra User đầu vào
                    var user = p as User;
                    if (user == null)
                    {
                        MessageBox.Show("Lỗi: Không lấy được thông tin dòng đã chọn (User is null).");
                        return;
                    }

                    // 2. Chuẩn bị dữ liệu cho Dialog
                    var dialog = new Views.Dialogs.Accounts.UpdateAccount();
                    var staffs = _staffRepository.GetAll();
                    if (staffs == null)
                    {
                        MessageBox.Show("Lỗi: Không tải được danh sách nhân viên từ CSDL.");
                        return;
                    }

                    var staffIds = staffs.Select(s => s.Id).ToList();
                    dialog.LoadStaffIds(staffIds);

                    // Gán StaffId hiện tại (Xử lý trường hợp StaffId bị null trong DB)
                    string currentStaffId = user.StaffId;
                    if (string.IsNullOrEmpty(currentStaffId))
                    {
                        // Nếu user này chưa có StaffId, hãy cẩn thận. 
                        // Có thể Service yêu cầu bắt buộc phải có StaffId.
                        // Tạm thời gán rỗng hoặc xử lý tùy logic của bạn.
                        currentStaffId = "";
                    }
                    else
                    {
                        currentStaffId = currentStaffId.Trim(); // Cắt khoảng trắng thừa
                    }

                    dialog.Account = currentStaffId;

                    // 3. Hiện Dialog
                    if (dialog.ShowDialog() == true)
                    {
                        string newPassword = dialog.Password;

                        // Validate mật khẩu
                        if (string.IsNullOrWhiteSpace(newPassword))
                        {
                            MessageBox.Show("Mật khẩu mới không được để trống.");
                            return;
                        }

                        // Lấy StaffId từ Dialog (Phòng trường hợp người dùng đổi Staff khác)
                        string targetStaffId = dialog.Account;

                        // DEBUG: Kiểm tra xem dữ liệu gửi đi là gì
                        if (string.IsNullOrEmpty(targetStaffId))
                        {
                            MessageBox.Show($"Lỗi: StaffID đang bị rỗng. User: {user.Username}");
                            return;
                        }

                        // 4. Tạo DTO và gọi Service
                        var updateDto = new DTOs.User.Requests.ChangePasswordRequestDto
                        {
                            NewPassword = newPassword,
                        };

                        // Gọi Service (Đảm bảo Trim() lần nữa cho chắc)
                        var result = _userService.ChangePassword(targetStaffId.Trim(), updateDto);

                        // 5. Kiểm tra kết quả trả về
                        if (!result.IsSuccess)
                        {
                            // QUAN TRỌNG: In ra ErrorMessage để biết tại sao lỗi
                            MessageBox.Show($"Service báo lỗi: {result.ErrorMessage}\n(Mã lỗi: {result.ErrorMessage})",
                                            "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        MessageBox.Show("Đổi mật khẩu thành công!");
                        LoadUsersFromDatabase();
                    }
                }
                catch (Exception ex)
                {
                    // Bắt lỗi crash chương trình (Exception)
                    MessageBox.Show($"Lỗi Crash ứng dụng: {ex.Message}\nStack Trace: {ex.StackTrace}",
                                    "Nghiêm trọng", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
            #endregion
            #endregion
            #region SearchCommand
            SearchUserCommand = new RelayCommand<object>(_ =>
            {
                if (string.IsNullOrWhiteSpace(SearchKeyword))
                {
                    LoadUsersFromDatabase();
                    return;
                }

                var result = _userService.SearchByUsername(SearchKeyword);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("lỗi khi tìm account");return;  
                }
                Users.Clear();

                foreach (var u in result.Data)
                {
                    Users.Add(new User
                    {
                        UserId = u.AccountId,
                        StaffId = u.StaffId,
                        Username = u.UserName,
                        UserRole = u.Role,
                        CreatedDate = u.CreateDate
                    });
                }

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

            #region PrintCommand 
            PrintCommand = new RelayCommand<object>((p) =>
            {
                var data = Users;
                var dialog = new PrintAccount(data);
                dialog.ShowDialog();
            });
            #endregion
            #region ExportCommand
            ExportCommand = new RelayCommand<object>((p) =>
            {
                var data = Users.Select(u => new UserResponseDto
                {
                    AccountId = u.UserId,
                    StaffId = u.StaffId,
                    UserName = u.Username,
                    Password = "******",
                    Role = u.UserRole
                }).ToList();

                var dialog = new ExportExelAccount(data);
                dialog.ShowDialog();
            });
            #endregion

        }
    }
}
