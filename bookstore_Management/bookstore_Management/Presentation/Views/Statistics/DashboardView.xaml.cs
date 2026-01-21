using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;


namespace bookstore_Management.Presentation.Views.Statistics
{

    public partial class DashboardView : UserControl
    {

        public DashboardView(DashboardViewModel dashboardViewModel)
        {
            InitializeComponent();
            this.DataContext = dashboardViewModel;

            dashboardViewModel.SelectedTimeRange = TimeRange.Today;
        }


    }
}