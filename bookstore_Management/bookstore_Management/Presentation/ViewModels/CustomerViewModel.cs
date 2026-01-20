using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using bookstore_Management.Presentation.Views.Dialogs.Customers;
using CommunityToolkit.Mvvm.Input;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class CustomerViewModel : BaseViewModel
    {
        private readonly ICustomerService _customerService;

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set { _customers = value; OnPropertyChanged(); }
        }

        private Customer _selectedCus;
        public Customer SelectedCus
        {
            get => _selectedCus;
            set { _selectedCus = value; OnPropertyChanged(); }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                SearchCusCommand?.Execute(null);
            }
        }

        public ICommand AddCusCommand { get; set; }
        public ICommand RemoveCusCommand { get; set; }
        public ICommand EditCusCommand { get; set; }
        public ICommand SearchCusCommand { get; set; }
        public ICommand LoadData { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        public CustomerViewModel(ICustomerService customerService)
        {
            _customerService = customerService;
            Customers = new ObservableCollection<Customer>();

            AddCusCommand = new AsyncRelayCommand(AddCommandAsync);
            EditCusCommand = new AsyncRelayCommand(EditCommandAsync);
            RemoveCusCommand = new AsyncRelayCommand(RemoveCommandAsync);
            SearchCusCommand = new AsyncRelayCommand(SearchCusCommandAsync);
            LoadData = new AsyncRelayCommand(LoadDataCommandAsync);
            PrintCommand = new AsyncRelayCommand(PrintCommandAsync);
            ExportCommand = new AsyncRelayCommand(ExportCommandAsync);
        }

        public async Task LoadCusFromDatabase()
        {
            var result = await _customerService.GetAllCustomersAsync();
            if (!result.IsSuccess)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu khách hàng: " + result.ErrorMessage);
                return;
            }
            if (result.Data == null) return;

            Customers = new ObservableCollection<Customer>(
                result.Data.Select(dto => new Customer
                {
                    CustomerId = dto.CustomerId,
                    Name = dto.Name,
                    Phone = dto.Phone,
                    Email = dto.Email
                })
            );
        }

        private async Task AddCommandAsync()
        {
            var dialog = new AddCustomer();
            if (dialog.ShowDialog() == true)
            {
                var dto = new DTOs.Customer.Requests.CreateCustomerRequestDto
                {
                    Name = dialog.CustomerName,
                    Address = dialog.Address,
                    Email = dialog.Email,
                    Phone = dialog.Phone
                };
                var result = await _customerService.AddCustomerAsync(dto);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi thêm khách hàng: " + result.ErrorMessage);
                    return;
                }
                await LoadCusFromDatabase();
            }
        }

        private async Task EditCommandAsync()
        {
            var cus = SelectedCus;
            if (cus == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng để sửa.");
                return;
            }

            var dialog = new UpdateCustomer
            {
                CustomerName = cus.Name,
                Phone = cus.Phone,
                Email = cus.Email,
                Address = cus.Address
            };

            if (dialog.ShowDialog() == true)
            {
                var dto = new DTOs.Customer.Requests.UpdateCustomerRequestDto
                {
                    Name = dialog.CustomerName,
                    Address = dialog.Address,
                    Email = dialog.Email,
                    Phone = dialog.Phone
                };
                var result = await _customerService.UpdateCustomerAsync(cus.CustomerId, dto);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Chi tiết lỗi: " + result.ErrorMessage);
                    return;
                }
                await LoadCusFromDatabase();
            }
        }

        private async Task RemoveCommandAsync()
        {
            var cus = SelectedCus;
            if (cus == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng để xóa.");
                return;
            }

            var confirmed = Views.Dialogs.Share.Delete.ShowForCustomer(cus.Name, cus.CustomerId);
            if (!confirmed) return;

            var result = await _customerService.DeleteCustomerAsync(cus.CustomerId);
            if (!result.IsSuccess)
            {
                MessageBox.Show("Không thể xóa khách hàng: " + result.ErrorMessage);
                return;
            }
            await LoadCusFromDatabase();
        }

        private async Task SearchCusCommandAsync()
        {
            if (string.IsNullOrEmpty(SearchKeyword))
            {
                await LoadCusFromDatabase();
                return;
            }

            var result = await _customerService.SearchByNameAsync(SearchKeyword);
            if (!result.IsSuccess)
            {
                MessageBox.Show("Lỗi khi tìm kiếm khách hàng: " + result.ErrorMessage);
                return;
            }
            if (result.Data == null) return;

            Customers = new ObservableCollection<Customer>(
                result.Data.Select(c => new Customer
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    Phone = c.Phone,
                    Email = c.Email
                })
            );
        }

        private async Task LoadDataCommandAsync()
        {
            SearchKeyword = string.Empty;
            await LoadCusFromDatabase();
        }

        private async Task PrintCommandAsync()
        {
            // TODO: in danh sách khách hàng
        }

        private async Task ExportCommandAsync()
        {
            // TODO: xuất Excel
        }
    }
}
