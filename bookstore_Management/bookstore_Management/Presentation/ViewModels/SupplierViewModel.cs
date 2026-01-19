using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;
using bookstore_Management.Presentation.Views.Dialogs.Publishers;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class PublisherViewModel : BaseViewModel
    {
        #region các khai báo
        private readonly IPublisherService _publisherService;
        private readonly IPublisherRepository _publisherRepository;

        //dữ liệu để view binding
        private ObservableCollection<Publisher> _publishers;
        public ObservableCollection<Publisher> Publishers
        {
            get { return _publishers; }
            set
            {
                _publishers = value;
                OnPropertyChanged();
            }
        }

        //sách đã chọn để xóa/sửa
        private Publisher _selectedPublisher;
        public Publisher SelectedPublisher
        {
            get => _selectedPublisher;
            set
            {
                _selectedPublisher = value;
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
                SearchPusCommand.Execute(null);
            }
        }
        #endregion

        #region Khai báo command
        //khai báo command cho thao tác thêm, xóa, sửa nxb
        public ICommand AddPusCommand { get; set; }
        public ICommand RemovePusCommand { get; set; }
        public ICommand EditPusCommand { get; set; }

        //command cho thao tác tìm kiếm - load lại
        public ICommand SearchPusCommand { get; set; }
        public ICommand LoadData { get; set; }

        //command cho in / xuất excel
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        #endregion

        #region Load publisher from db
        private void LoadPublishersFromDatabase()
        {
            var result = _publisherService.GetAllPublishers();
            if (!result.IsSuccess)
            {
                // Xử lý lỗi, để sau này làm thông báo lỗi sau
                return;
            }
            if (result.Data == null) return; // Tránh lỗi khi Data rỗng
            var publishers = result.Data.Select(dto => new Publisher
            {
                Id = dto.Id,
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                CreatedDate = dto.CreatedDate,

            });
            Publishers = new ObservableCollection<Publisher>(publishers);
        }
        #endregion

        #region constructor
        public PublisherViewModel(IPublisherService publisherService)
        {
            var context = new BookstoreDbContext();
            _publisherRepository = new Data.Repositories.Implementations.PublisherRepository(context);
            var bookRepo = new Data.Repositories.Implementations.BookRepository(context);
            var billRepo = new Data.Repositories.Implementations.ImportBillRepository(context);

            // 2. Truyền repository đã khởi tạo vào Service
            _publisherService = new PublisherService(
                _publisherRepository,
                bookRepo,
                billRepo
            );

            Publishers = new ObservableCollection<Publisher>();
            LoadPublishersFromDatabase();

            #region AddCommand
            AddPusCommand = new RelayCommand<object>((p) =>
            {
                var dialog = new Presentation.Views.Dialogs.Publishers.AddPublisher();
                if(dialog.ShowDialog() == true)
                {
                    var newPublisher = new DTOs.Publisher.Requests.CreatePublisherRequestDto
                    {
                        Name = dialog.PublisherName,
                        Phone = dialog.Phone,
                        Email = dialog.Email,
                    };
                    var result = _publisherService.AddPublisher(newPublisher);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Lỗi khi thêm nhà xuất bản: " + result.ErrorMessage, "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        // Xử lý lỗi, để sau này làm thông báo lỗi sau
                        return;
                    }
                    LoadPublishersFromDatabase();
                }
            });
            #endregion
            #region EditCommand
            EditPusCommand = new RelayCommand<object>((p) =>
            {
                var dialog = new Presentation.Views.Dialogs.Publishers.UpdatePublisher();
                var pus = p as Publisher;

                var publishers = _publisherRepository.GetAll();
                var publisherNames = publishers.Select(x => x.Name).ToList();

                // Nạp danh sách vào trước
                dialog.LoadPublisherData(
                    publisherNames,       // <--- Truyền list vào đây
                    pus.Id.ToString(),
                    pus.Name,
                    pus.Phone,
                    pus.Email,
                    // Nếu muốn truyền ngày thì thêm vào sau, không thì để null nó tự lấy mặc định
                    null,
                    null
                );
                if (pus == null) return;

                //dialog.PublisherId = pus.Id;
                dialog.PublisherName = pus.Name;
                dialog.Phone = pus.Phone;
                dialog.Email = pus.Email;

                if(dialog.ShowDialog() == true)
                {
                    var updatedPublisher = new DTOs.Publisher.Requests.UpdatePublisherRequestDto
                    {
                        Name = dialog.PublisherName,
                        Phone = dialog.Phone,
                        Email = dialog.Email,
                    };
                    var result = _publisherService.UpdatePublisher(pus.Id,updatedPublisher);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Lỗi khi cập nhật nhà xuất bản: " + result.ErrorMessage, "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        // Xử lý lỗi, để sau này làm thông báo lỗi sau
                        return;
                    }
                    LoadPublishersFromDatabase();
                }
            });
            #endregion
            #region RemoveCommand
            RemovePusCommand = new RelayCommand<object>((p) =>
            {
                var pus = p as Publisher;
                if (pus == null) return;

                bool comfirm = Views.Dialogs.Share.Delete.ShowForPublisher(pus.Name, pus.Id);
                if (!comfirm) return;

                var result = _publisherService.DeletePublisher(pus.Id);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi xóa nhà xuất bản: " + result.ErrorMessage, "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    // Xử lý lỗi, để sau này làm thông báo lỗi sau
                    return;
                }
                LoadPublishersFromDatabase();
            });
            #endregion
            #region SearchCommand
            SearchPusCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrWhiteSpace(SearchKeyword))
                {
                    LoadPublishersFromDatabase();
                }
                else
                {
                    var result = _publisherService.SearchByName(SearchKeyword);
                    if (!result.IsSuccess)
                    {
                        // Xử lý lỗi, để sau này làm thông báo lỗi sau
                        return;
                    }
                    Publishers.Clear();
                    foreach (var dto in result.Data)
                    {
                        Publishers.Add(new Publisher
                        {
                            Id = dto.Id,
                            Name = dto.Name,
                            Phone = dto.Phone,
                            Email = dto.Email,
                        });
                    }
                }
            });
            #endregion
            #region LoadDataCommand
            LoadData = new RelayCommand<object>((p) =>
            {
                SearchKeyword = string.Empty;
                LoadPublishersFromDatabase();
            });
            #endregion
            #region Print & Export
            PrintCommand = new RelayCommand<object>((p) =>
            {
                var data = Publishers;
                var dialog = new PrintPublisher(data);
                dialog.ShowDialog();
            });
            ExportCommand = new RelayCommand<object>((p) =>
            {

            });
            #endregion

        }
        #endregion
    }
}
