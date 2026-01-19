using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class StaffViewModel : BaseViewModel
    {
        #region các khai báo
        //lấy service
        private readonly IStaffService _staffService;

        //dữ liệu để view binding
        private ObservableCollection<Staff> _staffs;
        public ObservableCollection<Staff> Staffs
        {
            get { return _staffs; }
            set
            {
                _staffs = value;
                OnPropertyChanged();
            }
        }
       
        //sách đã chọn để xóa/sửa
        private Staff _selectedStaff;
        public Staff SelectedStafff
        {
            get => _selectedStaff;
            set
            {
                _selectedStaff = value;
                OnPropertyChanged();
            }
        }

        //keyword để tìm kiếm
        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                SearchStaffCommand.Execute(null);
            }
        }
        #endregion

        #region Khai báo command
        //khai báo command cho thao tác thêm, xóa, sửa staff
        public ICommand AddStaffCommand { get; set; }
        public ICommand RemoveStaffCommand { get; set; }
        public ICommand EditStaffCommand { get; set; }

        //command cho thao tác tìm kiếm - load lại
        public ICommand SearchStaffCommand { get; set; }
        public ICommand LoadData { get; set; }

        //command cho in / xuất excel
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        #endregion

        #region Load staff from db
        private async Task LoadStaffsFromDatabase()
        {
            var result = await _staffService.GetAllStaffAsync();
            if (!result.IsSuccess)
            {
                // Xử lý lỗi, để sau này làm thông báo lỗi sau
                MessageBox.Show("Lỗi khi tải dữ liệu nhân viên: " + result.ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (result.Data == null) return; // Tránh lỗi khi Data rỗng

            var staff = result.Data.Select(s => new Staff
            {
                Id = s.Id,
                Name = s.Name,
                CitizenId = s.CitizenId,
                Phone = s.Phone,
                UserRole = s.UserRole,
                CreatedDate = s.CreatedDate,
            }).ToList();

            Staffs = new ObservableCollection<Staff>(staff);
        }
        #endregion

  
        
        public StaffViewModel(IStaffService staffService)
        {
            var context = new BookstoreDbContext();   
            var unitOfWork = new  UnitOfWork(context);

            _staffService = new StaffService(unitOfWork);

            Staffs = new ObservableCollection<Staff>();
            _ = LoadStaffsFromDatabase();

            #region AddCommand
            AddStaffCommand = new RelayCommand<object>((p) =>
                Task.Run(async () =>
            {
                var dialog = new Views.Dialogs.Staffs.AddStaff();
                if (dialog.ShowDialog() == true)
                {
                    // Call service to add book to database
                    var newStaffDto = new DTOs.Staff.Requests.CreateStaffRequestDto
                    {
                        Name = dialog.StaffName,
                        CitizenId = dialog.CCCD,
                        Phone = dialog.PhoneNumber,
                        UserRole = dialog.Role,
                    };
                    var result = await _staffService.AddStaffAsync(newStaffDto);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Lỗi khi thêm sách: " + result.ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    // Reload books from database
                    _ = LoadStaffsFromDatabase();
                }
            }));
            #endregion
            #region RemoveCommand
            RemoveStaffCommand = new RelayCommand<object>((p) =>
                Task.Run(async () =>
            {
                var staff = p as Staff;
                if (staff == null)
                {
                    MessageBox.Show("Vui lòng chọn sách để xóa");
                    return;
                }

                bool confirmed = Views.Dialogs.Share.Delete.ShowForStaff(staff.Name, staff.Id);

                if (!confirmed) return;

                var result = await _staffService.DeleteStaffAsync(staff.Id);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi xóa sách: " + result.ErrorMessage,
                                    "Lỗi",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }
                _ = LoadStaffsFromDatabase();
            }));
            #endregion
            #region EditCommand
            EditStaffCommand = new RelayCommand<object>((p) =>
            Task.Run(async () =>
                {
                var dialog = new Views.Dialogs.Staffs.UpdateStaff();
                var staff = p as Staff;
                if (staff == null)
                {
                    MessageBox.Show("Vui lòng chọn sách để chỉnh sửa");
                    return;
                }
                
                dialog.StaffID = staff.Id;
                dialog.StaffName = staff.Name;
                dialog.Role= staff.UserRole;
                dialog.PhoneNumber = staff.Phone;
                dialog.CCCD = staff.CitizenId;

                if (dialog.ShowDialog() == true)
                {
                    var updateDto = new DTOs.Staff.Requests.UpdateStaffRequestDto()
                    {
                        Name = dialog.StaffName,
                        CitizenId = dialog.CCCD,
                        Phone = dialog.PhoneNumber,
                        UserRole = dialog.Role,
                    };

                    var result = await _staffService.UpdateStaffAsync(staff.Id, updateDto);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Lỗi khi cập nhật / chỉnh sửa sách");
                        return;
                    }

                    _ = LoadStaffsFromDatabase();
                }
            }));
            #endregion
            #region SearchCommand
            SearchStaffCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(SearchKeyword))
                {
                    _ = LoadStaffsFromDatabase();//k nhập gì thì hiện lại list
                    return;
                }

                var result = _staffService.SearchByName(SearchKeyword);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi tìm sách");
                    return;
                }
                Staffs.Clear();
                foreach (var s in result.Data)
                {
                    Staffs.Add(new Staff
                    {
                        Id = s.Id,
                        Name = s.Name,
                        CitizenId = s.CitizenId,
                        Phone = s.Phone,
                        UserRole = s.UserRole,
                        CreatedDate = s.CreatedDate,
                    });
                }
            });
            #endregion
            #region LoadDataCommand
            LoadData = new RelayCommand<object>((p) =>
            {
                SearchKeyword = string.Empty;
                LoadStaffsFromDatabase();
            });
            #endregion

            //chưa làm xong
            #region PrintCommand 
            PrintCommand = new RelayCommand<object>((p) =>
            {

            });
            #endregion
            #region ExportCommand
            ExportCommand = new RelayCommand<object>((p) =>
            {

            });
            #endregion
        }
    }
}
