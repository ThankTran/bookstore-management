using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Services.Interfaces;
using System.Linq;
using System.Windows.Controls;
using bookstore_Management.Services.Implementations;

namespace bookstore_Management.Presentation.Views.Users
{
    /// <summary>
    /// Interaction logic for StaffListView.xaml
    /// </summary>
    public partial class StaffListView : UserControl
    {
        private StaffViewModel _viewModel;
        public StaffListView() 
        {
            InitializeComponent();
            IStaffService staffService;

            var context = new Data.Context.BookstoreDbContext();
            staffService = new StaffService(
                new Data.Repositories.Implementations.StaffRepository(context),
                new Data.Repositories.Implementations.OrderRepository(context)
            );

            _viewModel = new StaffViewModel(staffService);
            this.DataContext = _viewModel;
        }

        private void dgStaff_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
