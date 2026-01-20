using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using bookstore_Management.DTOs.Order.Responses;
using DocumentFormat.OpenXml.Wordprocessing;

namespace bookstore_Management.Presentation.Views.Dialogs.Payment
{
    /// <summary>
    /// Dialog hiển thị và in hóa đơn thanh toán
    /// </summary>
    public partial class Pay : Window
    {
        public bool IsConfirmed { get; private set; }

        public Pay(object dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}