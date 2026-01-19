using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Services.Interfaces;
using System.Linq;
using System.Windows.Controls;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
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
            
            var context = new BookstoreDbContext();
            var unitOfWork = new UnitOfWork(context);
            IStaffService staffService = new StaffService(unitOfWork);

            _viewModel = new StaffViewModel(staffService);
            this.DataContext = _viewModel;
        }

        private void dgStaff_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
