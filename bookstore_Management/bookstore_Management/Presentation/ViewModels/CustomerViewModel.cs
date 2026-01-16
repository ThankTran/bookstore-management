using bookstore_Management.Models;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.ViewModels
{
    internal class CustomerViewModel : BaseViewModel
    {
        #region khai báo
        private readonly ICustomerService _customerService = new CustomerService();

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
        private string _searchKeywork;
        public string SearchKeywork
        {
            get => _searchKeywork;
            set
            {
                _searchKeywork = value;
                OnPropertyChanged();
                //SearchBookCommand.Execute(null);
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
                // Xử lý lỗi, để sau này làm thông báo lỗi sau
                MessageBox.Show("Lỗi khi tải dữ liệu khách hàng: " + result.ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (result.Data == null) return; // Tránh lỗi khi Data rỗng

            var cuss = result.Data.Select(dto => new Customer
            {
                  Name = dto.Name,
                  Phone = dto.Phone,
                  
            });

            Customers = new ObservableCollection<Customer>(cuss);
        }
        #endregion

    }
}
