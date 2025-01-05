using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ConvenienceStore.ViewModels;
using ConvenienceStore.Services;
using System;
using System.Diagnostics;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ConvenienceStore.Views
{
    public sealed partial class RevenueReportPage : Page
    {
        public RevenueReportViewModel ViewModel { get; }

        public RevenueReportPage()
        {
            string connectionString = @"Data Source=.\SQL22;Initial Catalog=ConvenienceStoreDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            var databaseService = new RevenueReportDatabaseService(connectionString);
            ViewModel = new RevenueReportViewModel(databaseService);

            this.InitializeComponent();

            // Initialize TimeRangeComboBox
            TimeRangeComboBox.ItemsSource = new string[] { "Tháng", "Năm" };
            TimeRangeComboBox.SelectedItem = "Tháng";

            // Set DataContext to ViewModel
            DataContext = ViewModel;


        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await LoadDataWithErrorHandling();
        }

        private async Task LoadDataWithErrorHandling()
        {
            try
            {
                await ViewModel.LoadData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");

                var errorDialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Không thể tải dữ liệu. Vui lòng thử lại sau.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await errorDialog.ShowAsync();
            }
        }
    }
}