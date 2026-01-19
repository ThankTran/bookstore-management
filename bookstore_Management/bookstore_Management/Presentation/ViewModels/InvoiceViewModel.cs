using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace bookstore_Management.Presentation.ViewModels
{
    internal class InvoiceViewModel : BaseViewModel
    {
        #region khai báo
        private readonly IOrderService _orderService;
        private readonly IImportBillService _importBillService;


        private ObservableCollection<ImportBill> _imports;
        public ObservableCollection<ImportBill> Imports
        {
            get { return _imports; }
            set
            {
                _imports = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                OnPropertyChanged();
            }
        }

        private Order _selectedInvoice;
        public Order SelectedInvoice
        {
            get => _selectedInvoice;
            set
            {
                _selectedInvoice = value;
                OnPropertyChanged();
            }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                SearchInvoiceCommand.Execute(null);
            }
        }
        #endregion

        #region khai báo command
        //khai báo command cho thao tác thêm, xóa, sửa hóa đơn
        public ICommand AddOrderCommand { get; set; }
        public ICommand AddImportCommand { get; set; }
        public ICommand RemoveInvoiceCommand { get; set; }
        public ICommand EditInvoiceCommand { get; set; }

        //command cho thao tác tìm kiếm - load lại
        public ICommand SearchInvoiceCommand { get; set; }

        //command cho in / xuất excel
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        #endregion

        #region Load data from db
        private void LoadOrdersFromDatabase()
        {
            var result = _orderService.GetAllOrders();
            if(!result.IsSuccess)
            {
                MessageBox.Show("Lỗi tải đơn hàng: " + result.ErrorMessage);
                return;
            }
            if(result.Data == null) return; 

            var orders = result.Data.Select(o => new Order
            {
                OrderId = o.OrderId,
                StaffId = o.StaffId,
                CustomerId = o.CustomerId,
                PaymentMethod = o.PaymentMethod,
                Discount = o.Discount,
                TotalPrice = o.TotalPrice,
                Notes = o.Notes,
                CreatedDate = o.CreatedDate,
            }).ToList();

            Orders = new ObservableCollection<Order>(orders);
        }

        private void LoadImportsFromDatabase()
        {
            var result = _importBillService.GetAllImportBills();
            if (!result.IsSuccess)
            {
                MessageBox.Show("Lỗi tải hóa đơn nhập: " + result.ErrorMessage);
                return;
            }
            if (result.Data == null) return;
            var imports = result.Data.Select(i => new ImportBill
            {
                Id = i.Id,
                PublisherId = i.PublisherId,
                TotalAmount = i.TotalAmount,
                Notes = i.Notes,
                CreatedBy = i.CreatedBy,
                CreatedDate = i.CreatedDate,

            }).ToList();
            Imports = new ObservableCollection<ImportBill>(imports);
        }
        private void LoadAllData()
        {
            LoadOrdersFromDatabase();
            LoadImportsFromDatabase();
        }
        #endregion

        #region constructor
        public InvoiceViewModel(IImportBillService importBillService, IOrderService orderService)
        {
            //hóa đơn nhập
            var context1 = new BookstoreDbContext();
            _importBillService = new ImportBillService(
            new ImportBillRepository(context1),
            new ImportBillDetailRepository(context1),
            new BookRepository(context1),
            new PublisherRepository(context1)
            );

            //hóa đơn xuất
            var context2 = new BookstoreDbContext();
            _orderService = new OrderService(
            new OrderRepository(context2),
            new OrderDetailRepository(context2),
            new BookRepository(context2),
            new CustomerRepository(context2),
            new StaffRepository(context2)
            );

            Imports = new ObservableCollection<ImportBill>();
            Orders = new ObservableCollection<Order>();

            LoadAllData();

            #region AddCommand
            AddImportCommand = new RelayCommand<object>((p) => 
            {
                var dialog = new Views.Dialogs.Invoices.CreateImportBill();
                if(dialog.ShowDialog() == true)
                {
                    var newImportDto = new DTOs.ImportBill.Requests.CreateImportBillRequestDto
                    {
                        PublisherId = dialog.PublisherId,
                        Notes = dialog.Notes,
                        CreatedBy = dialog.CreatedBy,
                        //ImportBillDeatail
                    };

                    var result = _importBillService.CreateImportBill(newImportDto);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Lỗi khi thêm hóa đơn nhập");
                        return;
                    }

                    LoadAllData();
                }
            });
            AddOrderCommand = new RelayCommand<object>((p) =>
            {
                var dialog = new Views.Dialogs.Invoices.CreateOrderBill();
                if (dialog.ShowDialog() == true)
                {
                    var newOrderDto = new DTOs.Order.Requests.CreateOrderRequestDto
                    {
                        //StaffId = dialog.id
                        //CustomerId = dialog.CustomerId,
                        //PaymentMethod = dialog.PaymentMethod,
                        //Discount = dialog.Discount,
                        //Notes = dialog.Notes,
                        //OrderDetail
                    };
                    LoadAllData();
                }
            });
            #endregion
            #region EditCommand
            EditInvoiceCommand = new RelayCommand<object>((p) =>
            {

            });
            #endregion
            #region RemoveCommand
            RemoveInvoiceCommand = new RelayCommand<object>((p) =>
            {
                
            });
            #endregion
            #region SearchCommand
            SearchInvoiceCommand = new RelayCommand<object>((p) =>
            {

            });
            #endregion
            #region Print&Export

            #endregion

        }
        #endregion
    }
}
