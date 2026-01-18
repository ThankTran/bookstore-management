using System;
using System.Windows;
using System.Windows.Controls;
using bookstore_Management.Presentation.ViewModels;

namespace bookstore_Management.Presentation.Views.Statistics
{

    public partial class DashboardView : UserControl
    {
        private DashboardViewModel _viewModel;

        public DashboardView()
        {
            InitializeComponent();
            //_viewModel = new DashboardViewModel();
            //this.DataContext = _viewModel;
        }

        //public void RefreshData()
        //{
        //    _viewModel.RefreshDashboard();
        //}
    }
}