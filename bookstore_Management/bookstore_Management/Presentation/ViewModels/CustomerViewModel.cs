using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Models;
using bookstore_Management.Presentation.Views.Dialogs.Customers;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using DocumentFormat.OpenXml.VariantTypes;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class CustomerViewModel : BaseViewModel
    {
        #region khai báo
        private readonly ICustomerService _customerService;

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers
        {
            get { return _customers; }
            set 
            {
                _customers = value; 
                OnPropertyChanged();
            }
        }

        //cus đã chọn để xóa/sửa
        private Customer _selectedCus;
        public Customer SelectedCus
        {
            get => _selectedCus;
            set
            {
                _selectedCus = value;
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
                SearchCusCommand.Execute(null);
            }
        }
        #endregion

        #region Khai báo command
        //khai báo command cho thao tác thêm, xóa, sửa cus
        public ICommand AddCusCommand { get; set; }
        public ICommand RemoveCusCommand { get; set; }
        public ICommand EditCusCommand { get; set; }

        //command cho thao tác tìm kiếm - load lại
        public ICommand SearchCusCommand { get; set; }
        public ICommand LoadData { get; set; }

        //command cho in / xuất excel
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        #endregion

        #region load cus from database
        public void LoadCusFromDatabase()
        {
            var result = _customerService.GetAllCustomers();

            if (!result.IsSuccess)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu khách hàng: " + result.ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (result.Data == null)
            {
                MessageBox.Show("db không có dữ liệu!");
                return; 
            }               

            var cuss = result.Data.Select(dto => new Customer
            {
                  CustomerId = dto.CustomerId,
                  Name = dto.Name,
                  Phone = dto.Phone,  
                  Email = dto.Email,
                  Address = dto.Address,
            });

            Customers = new ObservableCollection<Customer>(cuss);
        }
        #endregion

        public CustomerViewModel(ICustomerService customerService)
        {
            //_customerService = customerService??new CustomerService();
            var context = new BookstoreDbContext();

            _customerService = new CustomerService(
            new CustomerRepository(context),
            new OrderRepository(context)
            );

            Customers = new ObservableCollection<Customer>();

            LoadCusFromDatabase();

            #region AddCommand
            AddCusCommand = new RelayCommand<object>((p) =>
            {
                var dialog = new Presentation.Views.Dialogs.Customers.AddCustomer();
                if (dialog.ShowDialog() == true)
                {
                    var newCusDto = new DTOs.Customer.Requests.CreateCustomerRequestDto()
                    {
                        Name=dialog.CustomerName,
                        Address=dialog.Address,
                        Email=dialog.Email,
                        Phone=dialog.Phone,
                    };
                    var result = _customerService.AddCustomer(newCusDto);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Lỗi khi thêm khách hàng");
                        return;
                    }
                    LoadCusFromDatabase();
                }
            });
            #endregion
            #region EditCommand
            EditCusCommand = new RelayCommand<object>((p) => 
            { 
                var dialog = new Presentation.Views.Dialogs.Customers.UpdateCustomer();
                var cus = p as Customer;
                if (cus == null)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng để sửa.");
                    return;
                }
                dialog.CustomerName = cus.Name;
                dialog.Phone = cus.Phone;
                dialog.Email = cus.Email;
                dialog.Address = cus.Address;
                if (dialog.ShowDialog() == true)
                {
                    var updateCusDto = new DTOs.Customer.Requests.UpdateCustomerRequestDto()
                    {
                        Name = dialog.CustomerName,
                        Address = dialog.Address,
                        Email = dialog.Email,
                        Phone = dialog.Phone,
                    };
                    var result = _customerService.UpdateCustomer(cus.CustomerId, updateCusDto);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show($"Chi tiết lỗi: {result.ErrorMessage}", "Lỗi thêm khách hàng", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    LoadCusFromDatabase();
                }
            });
            #endregion
            #region RemoveCommand
            RemoveCusCommand = new RelayCommand<object>((p) => 
            {
                var cus = p as Customer;
                if (cus == null)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng để xóa.");
                    return;
                }
                bool confirmed = Views.Dialogs.Share.Delete.ShowForCustomer(cus.Name, cus.CustomerId);
                if (!confirmed) return;
                
                var result = _customerService.DeleteCustomer(cus.CustomerId);
                if (!result.IsSuccess)
                {
                    MessageBox.Show($"Không thể xóa khách hàng.\nChi tiết lỗi: {result.ErrorMessage}",
                        "Lỗi xóa dữ liệu",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                LoadCusFromDatabase();
            });
            #endregion
            #region SearchCommand
            SearchCusCommand = new RelayCommand<object>((p)=> 
            {
                if (string.IsNullOrEmpty(SearchKeyword))
                {
                    LoadCusFromDatabase();
                    return;
                }
                var result = _customerService.SearchByName(SearchKeyword);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi tìm kiếm khách hàng: " + result.ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (result.Data == null)
                {
                    MessageBox.Show("db không có dữ liệu!");
                    return; // Tránh lỗi khi Data rỗng
                }
                Customers.Clear();
                foreach(var c in result.Data)
                {
                    Customers.Add(new Customer
                    {
                        CustomerId = c.CustomerId,
                        Name = c.Name,
                        Phone = c.Phone,
                        Email = c.Email,
                    });
                }
            });
            #endregion
            #region LoadDataCommand
            LoadData = new RelayCommand<object>((p) =>
            {
                SearchKeyword = string.Empty;
                LoadCusFromDatabase();
            });
            #endregion
            #region Print & Export
            PrintCommand = new RelayCommand<object>((p)=> 
            {
                var data = Customers;
                var dialog = new PrintCustomer(data);
                dialog.ShowDialog();
            });
            ExportCommand = new RelayCommand<object>((p) => { });
            #endregion
        }
    }
}
