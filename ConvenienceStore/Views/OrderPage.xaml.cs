using ConvenienceStore.Services;
using ConvenienceStore.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;

namespace ConvenienceStore.Views
{
    public sealed partial class OrderPage : Page
    {
        public OrderViewModel ViewModel { get; }

        public OrderPage()
        {
            string connectionString = @"Data Source=.\SQL22;Initial Catalog=ConvenienceStoreDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            var databaseService = new OrderDatabaseService(connectionString);
            ViewModel = new OrderViewModel(databaseService);

            this.InitializeComponent();
            this.DataContext = ViewModel;
            this.Loaded += OrderPage_Loaded;
        }

        private async void OrderPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await ViewModel.LoadData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }


        private void SearchOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            // Chỉ thực hiện tìm kiếm khi người dùng nhấn nút tìm kiếm
            string searchQuery = SearchBox.Text;
            ViewModel.SearchOrdersByKeyword(searchQuery);
        }
        private void SearchPurchaseOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            // Chỉ thực hiện tìm kiếm khi người dùng nhấn nút tìm kiếm
            string searchQuery = SearchBox.Text;
            ViewModel.SearchPurchaseOrdersByKeyword(searchQuery);
        }
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PreviousPage();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NextPage();
        }
    }
}