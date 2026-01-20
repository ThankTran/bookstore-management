using bookstore_Management.Data.Context;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Presentation.Views.Dialogs.Publishers;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class PublisherViewModel : BaseViewModel
    {
        private readonly IPublisherService _publisherService;

        private ObservableCollection<Publisher> _publishers;
        public ObservableCollection<Publisher> Publishers
        {
            get => _publishers;
            set { _publishers = value; OnPropertyChanged(); }
        }

        private Publisher _selectedPublisher;
        public Publisher SelectedPublisher
        {
            get => _selectedPublisher;
            set { _selectedPublisher = value; OnPropertyChanged(); }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                SearchPusCommand?.Execute(null);
            }
        }

        public ICommand AddPusCommand { get; set; }
        public ICommand RemovePusCommand { get; set; }
        public ICommand EditPusCommand { get; set; }
        public ICommand SearchPusCommand { get; set; }
        public ICommand LoadData { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        public PublisherViewModel(IPublisherService publisherService)
        {
            _publisherService = publisherService;
            Publishers = new ObservableCollection<Publisher>();

            AddPusCommand = new AsyncRelayCommand(AddCommandAsync);
            EditPusCommand = new AsyncRelayCommand(EditCommandAsync);
            RemovePusCommand = new AsyncRelayCommand(RemoveCommandAsync);
            SearchPusCommand = new AsyncRelayCommand(SearchCommandAsync);
            LoadData = new AsyncRelayCommand(LoadDataCommandAsync);
            PrintCommand = new AsyncRelayCommand(PrintCommandAsync);
            ExportCommand = new AsyncRelayCommand(ExportCommandAsync);
        }

        public async Task LoadPublishersFromDatabaseAsync()
        {
            var result = await _publisherService.GetAllPublishersAsync();
            if (!result.IsSuccess)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu NXB: " + result.ErrorMessage);
                return;
            }
            if (result.Data == null) return;

            Publishers = new ObservableCollection<Publisher>(
                result.Data.Select(dto => new Publisher
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    CreatedDate = dto.CreatedDate
                })
            );
        }

        private async Task AddCommandAsync()
        {
            var dialog = new AddPublisher();
            if (dialog.ShowDialog() == true)
            {
                var dto = new DTOs.Publisher.Requests.CreatePublisherRequestDto
                {
                    Name = dialog.PublisherName,
                    Phone = dialog.Phone,
                    Email = dialog.Email
                };
                var result = await _publisherService.AddPublisherAsync(dto);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi thêm NXB: " + result.ErrorMessage);
                    return;
                }
                await LoadPublishersFromDatabaseAsync();
            }
        }

        private async Task EditCommandAsync()
        {
            var pus = SelectedPublisher;
            if (pus == null)
            {
                MessageBox.Show("Vui lòng chọn NXB để sửa.");
                return;
            }

            var dialog = new UpdatePublisher
            {
                PublisherName = pus.Name,
                Phone = pus.Phone,
                Email = pus.Email
            };

            if (dialog.ShowDialog() == true)
            {
                var dto = new DTOs.Publisher.Requests.UpdatePublisherRequestDto
                {
                    Name = dialog.PublisherName,
                    Phone = dialog.Phone,
                    Email = dialog.Email
                };
                var result = await _publisherService.UpdatePublisherAsync(pus.Id, dto);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi cập nhật NXB: " + result.ErrorMessage);
                    return;
                }
                await LoadPublishersFromDatabaseAsync();
            }
        }

        private async Task RemoveCommandAsync()
        {
            var pus = SelectedPublisher;
            if (pus == null)
            {
                MessageBox.Show("Vui lòng chọn NXB để xóa.");
                return;
            }

            bool confirm = Views.Dialogs.Share.Delete.ShowForPublisher(pus.Name, pus.Id);
            if (!confirm) return;

            var result = await _publisherService.DeletePublisherAsync(pus.Id);
            if (!result.IsSuccess)
            {
                MessageBox.Show("Không thể xóa NXB: " + result.ErrorMessage);
                return;
            }
            await LoadPublishersFromDatabaseAsync();
        }

        private async Task SearchCommandAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                await LoadPublishersFromDatabaseAsync();
                return;
            }

            var result = await _publisherService.SearchByNameAsync(SearchKeyword);
            if (!result.IsSuccess)
            {
                MessageBox.Show("Lỗi khi tìm NXB: " + result.ErrorMessage);
                return;
            }
            if (result.Data == null) return;

            Publishers = new ObservableCollection<Publisher>(
                result.Data.Select(dto => new Publisher
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Phone = dto.Phone,
                    Email = dto.Email
                })
            );
        }

        private async Task LoadDataCommandAsync()
        {
            SearchKeyword = string.Empty;
            await LoadPublishersFromDatabaseAsync();
        }

        private async Task PrintCommandAsync()
        {
            // TODO: in danh sách NXB
        }

        private async Task ExportCommandAsync()
        {
            // TODO: xuất Excel
        }
    }
}
