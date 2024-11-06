using ConvenienceStore.Services;
using ConvenienceStore.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ConvenienceStore.Models;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ConvenienceStore.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BillingPage : Page
    {
        public BillingViewModel ViewModel { get; }

        public BillingPage()
        {
            string connectionString = @"Data Source=DESKTOP-LD18TI4;Initial Catalog=ConvenienceStoreDB;Integrated Security=True;TrustServerCertificate=True";
            var databaseService = new DatabaseService(connectionString);
            ViewModel = new BillingViewModel(databaseService);

            this.InitializeComponent();

            // Đăng ký các event handlers
            this.Loaded += BillingPage_Loaded;
        }

        private async void BillingPage_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Page Loaded event fired");
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // Xử lý khi navigate đến page này
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            // Cleanup khi rời khỏi page
        }

        private async void AddBill_Click(object sender, RoutedEventArgs e)
        {
            // Thêm Bill mới
            var dialog = new ContentDialog
            {
                Title = "Thêm đơn hàng mới",
                PrimaryButtonText = "Thêm",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var panel = new StackPanel();
            var supplierIDTextBox = new TextBox { PlaceholderText = "Nhập ID supplier" };
            var orderDateTextBox = new DatePicker { };
            var totalAmountTextBox = new TextBox { PlaceholderText = "Nhập tổng tiền" };

            panel.Children.Add(supplierIDTextBox);
            panel.Children.Add(orderDateTextBox);
            panel.Children.Add(totalAmountTextBox);

            dialog.Content = panel;
            dialog.DataContext = dialog;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                var newBill = new Bill
                {
                    SupplierID = int.Parse(supplierIDTextBox.Text),
                    OrderDate = orderDateTextBox.Date.DateTime,
                    TotalAmount = int.Parse(totalAmountTextBox.Text)
                };
            }
        }

        private async void UpdateBill_Click(object sender, RoutedEventArgs e)
        {
            // Cập nhật Bill
            var dialog = new ContentDialog
            {
                Title = "Cập nhật đơn hàng",
                PrimaryButtonText = "Cập nhật",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var panel = new StackPanel();
            var purchaseOrderIDTextBox = new TextBox { PlaceholderText = "Nhập ID đơn hàng" };
            var supplierIDTextBox = new TextBox { PlaceholderText = "Nhập ID supplier" };
            var orderDateTextBox = new DatePicker { };
            var totalAmountTextBox = new TextBox { PlaceholderText = "Nhập tổng tiền" };

            panel.Children.Add(purchaseOrderIDTextBox);
            panel.Children.Add(supplierIDTextBox);
            panel.Children.Add(orderDateTextBox);
            panel.Children.Add(totalAmountTextBox);

            dialog.Content = panel;
            dialog.DataContext = dialog;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                var updatedBill = new Bill
                {
                    PurchaseOrderID = int.Parse(purchaseOrderIDTextBox.Text),
                    SupplierID = int.Parse(supplierIDTextBox.Text),
                    OrderDate = orderDateTextBox.Date.DateTime,
                    TotalAmount = int.Parse(totalAmountTextBox.Text)
                };
            }
        }

        private async void DeleteBill_Click(object sender, RoutedEventArgs e)
        {
            // Xóa Bill
            var dialog = new ContentDialog
            {
                Title = "Xóa đơn hàng",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var panel = new StackPanel();
            var purchaseOrderIDTextBox = new TextBox { PlaceholderText = "Nhập ID đơn hàng" };

            panel.Children.Add(purchaseOrderIDTextBox);

            dialog.Content = panel;
            dialog.DataContext = dialog;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                var purchaseOrderID = int.Parse(purchaseOrderIDTextBox.Text);
            }
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
