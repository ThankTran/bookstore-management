using bookstore_Management.Data.Context;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Presentation.Views.Dialogs.Staffs;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class StaffViewModel : BaseViewModel
    {
        private readonly IStaffService _staffService;

        private ObservableCollection<Staff> _staffs;
        public ObservableCollection<Staff> Staffs
        {
            get => _staffs;
            set { _staffs = value; OnPropertyChanged(); }
        }

        private Staff _selectedStaff;
        public Staff SelectedStaff
        {
            get => _selectedStaff;
            set { _selectedStaff = value; OnPropertyChanged(); }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                SearchStaffCommand?.Execute(null);
            }
        }

        public ICommand AddStaffCommand { get; set; }
        public ICommand RemoveStaffCommand { get; set; }
        public ICommand EditStaffCommand { get; set; }
        public ICommand SearchStaffCommand { get; set; }
        public ICommand LoadData { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        public StaffViewModel(IStaffService staffService)
        {
            _staffService = staffService;
            Staffs = new ObservableCollection<Staff>();

            AddStaffCommand = new AsyncRelayCommand(AddCommandAsync);
            EditStaffCommand = new AsyncRelayCommand(EditCommandAsync);
            RemoveStaffCommand = new AsyncRelayCommand(RemoveCommandAsync);
            SearchStaffCommand = new AsyncRelayCommand(SearchCommandAsync);
            LoadData = new AsyncRelayCommand(LoadDataCommandAsync);
            PrintCommand = new AsyncRelayCommand(PrintCommandAsync);
            ExportCommand = new AsyncRelayCommand(ExportCommandAsync);
        }

        private async Task LoadStaffsFromDatabase()
        {
            var result = await _staffService.GetAllStaffAsync();
            if (!result.IsSuccess)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu nhân viên: " + result.ErrorMessage);
                return;
            }
            if (result.Data == null) return;

            Staffs = new ObservableCollection<Staff>(
                result.Data.Select(s => new Staff
                {
                    Id = s.Id,
                    Name = s.Name,
                    CitizenId = s.CitizenId,
                    Phone = s.Phone,
                    UserRole = s.UserRole,
                    CreatedDate = s.CreatedDate
                })
            );
        }

        private async Task AddCommandAsync()
        {
            var dialog = new AddStaff();
            if (dialog.ShowDialog() == true)
            {
                var dto = new DTOs.Staff.Requests.CreateStaffRequestDto
                {
                    Name = dialog.StaffName,
                    CitizenId = dialog.CCCD,
                    Phone = dialog.PhoneNumber,
                    UserRole = dialog.Role
                };
                var result = await _staffService.AddStaffAsync(dto);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi thêm nhân viên: " + result.ErrorMessage);
                    return;
                }
                await LoadStaffsFromDatabase();
            }
        }

        private async Task EditCommandAsync()
        {
            var staff = SelectedStaff;
            if (staff == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên để chỉnh sửa");
                return;
            }

            var dialog = new UpdateStaff
            {
                StaffID = staff.Id,
                StaffName = staff.Name,
                Role = staff.UserRole,
                PhoneNumber = staff.Phone,
                CCCD = staff.CitizenId
            };

            if (dialog.ShowDialog() == true)
            {
                var dto = new DTOs.Staff.Requests.UpdateStaffRequestDto
                {
                    Name = dialog.StaffName,
                    CitizenId = dialog.CCCD,
                    Phone = dialog.PhoneNumber,
                    UserRole = dialog.Role
                };
                var result = await _staffService.UpdateStaffAsync(staff.Id, dto);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi cập nhật nhân viên: " + result.ErrorMessage);
                    return;
                }
                await LoadStaffsFromDatabase();
            }
        }

        private async Task RemoveCommandAsync()
        {
            var staff = SelectedStaff;
            if (staff == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên để xóa");
                return;
            }

            bool confirmed = Views.Dialogs.Share.Delete.ShowForStaff(staff.Name, staff.Id);
            if (!confirmed) return;

            var result = await _staffService.DeleteStaffAsync(staff.Id);
            if (!result.IsSuccess)
            {
                MessageBox.Show("Không thể xóa nhân viên: " + result.ErrorMessage);
                return;
            }
            await LoadStaffsFromDatabase();
        }

        private async Task SearchCommandAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                await LoadStaffsFromDatabase();
                return;
            }

            var result = await _staffService.SearchByNameAsync(SearchKeyword);
            if (!result.IsSuccess)
            {
                MessageBox.Show("Lỗi khi tìm nhân viên");
                return;
            }
            if (result.Data == null) return;

            Staffs = new ObservableCollection<Staff>(
                result.Data.Select(s => new Staff
                {
                    Id = s.Id,
                    Name = s.Name,
                    CitizenId = s.CitizenId,
                    Phone = s.Phone,
                    UserRole = s.UserRole,
                    CreatedDate = s.CreatedDate
                })
            );
        }

        private async Task LoadDataCommandAsync()
        {
            SearchKeyword = string.Empty;
            await LoadStaffsFromDatabase();
        }

        private async Task PrintCommandAsync()
        {
            var data = Staffs;
            var dialog = new PrintStaff(data);
            dialog.ShowDialog();
        }

        private async Task ExportCommandAsync()
        {
            // TODO: xuất Excel
        }
    }
}
