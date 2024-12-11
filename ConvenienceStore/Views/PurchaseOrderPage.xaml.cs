using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ConvenienceStore.ViewModels;
using ConvenienceStore.Models;
using ConvenienceStore.Services;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ConvenienceStore.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PurchaseOrderPage : Page
    {
        public PurchaseOrderViewModel ViewModel { get; }

        public PurchaseOrderPage()
        {
            string connectionString = @"Data Source=DESKTOP-LD18TI4;Initial Catalog=ConvenienceStoreDB;Integrated Security=True;TrustServerCertificate=True";
            var databaseService = new PurchaseOrderDatabaseService(connectionString);
            ViewModel = new PurchaseOrderViewModel(databaseService);

            this.InitializeComponent();
            this.Loaded += PurchaseOrderPage_Loaded;
        }

        private async void PurchaseOrderPage_Loaded(object sender, RoutedEventArgs e)
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

        private void PurchaseOrderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedPurchaseOrder = PurchaseOrderDataGrid.SelectedItem as PurchaseOrder;
        }

        private async void PurchaseOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Chi tiết đơn hàng",
                CloseButtonText = "Đóng",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot,
                MinWidth = 400
            };

            var stackPanel = new StackPanel();
            var listView = new ListView
            {
                ItemsSource = ViewModel.SelectedPurchaseOrderDetails,
                ItemTemplate = (DataTemplate)this.Resources["PurchaseOrderDetailTemplate"],
                SelectionMode = ListViewSelectionMode.None
            };

            stackPanel.Children.Add(listView);
            dialog.Content = stackPanel;

            await dialog.ShowAsync();
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentPage--;
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentPage++;
        }

        private async void AddPurchaseOrder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Thêm đơn hàng mới",
                PrimaryButtonText = "Thêm",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot,
                MinWidth = 400
            };

            var stackPanel = new StackPanel();

            var customerIDTextBox = new TextBox
            {
                Header = "Mã khách hàng",
                PlaceholderText = "Nhập mã khách hàng"
            };

            var orderDateDatePicker = new DatePicker
            {
                Header = "Ngày đặt hàng",
                Date = DateTime.Now
            };

            var totalAmountTextBox = new TextBox
            {
                Header = "Tổng tiền",
                PlaceholderText = "Nhập tổng tiền"
            };

            stackPanel.Children.Add(customerIDTextBox);
            stackPanel.Children.Add(orderDateDatePicker);
            stackPanel.Children.Add(totalAmountTextBox);

            dialog.Content = stackPanel;

            dialog.PrimaryButtonClick += async (s, args) =>
            {
                var newPurchaseOrder = new PurchaseOrder
                {
                    SupplierID = int.Parse(customerIDTextBox.Text),
                    OrderDate = orderDateDatePicker.Date.DateTime,
                    TotalAmount = decimal.Parse(totalAmountTextBox.Text)
                };

                await ViewModel.AddPurchaseOrder(newPurchaseOrder);
            };

            await dialog.ShowAsync();
        }

        private async void DeletePurchaseOrder_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedPurchaseOrder != null)
            {
                await ViewModel.DeletePurchaseOrder(ViewModel.SelectedPurchaseOrder);
            }
        }

        private async void UpdatePurchaseOrder_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedPurchaseOrder != null)
            {
                var dialog = new ContentDialog
                {
                    Title = "Cập nhật đơn hàng",
                    PrimaryButtonText = "Cập nhật",
                    CloseButtonText = "Hủy",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot,
                    MinWidth = 400
                };

                var stackPanel = new StackPanel();

                var customerIDTextBox = new TextBox
                {
                    Header = "Mã khách hàng",
                    PlaceholderText = "Nhập mã khách hàng",
                    Text = ViewModel.SelectedPurchaseOrder.SupplierID.ToString()
                };

                var orderDateDatePicker = new DatePicker
                {
                    Header = "Ngày đặt hàng",
                    Date = ViewModel.SelectedPurchaseOrder.OrderDate
                };

                var totalAmountTextBox = new TextBox
                {
                    Header = "Tổng tiền",
                    PlaceholderText = "Nhập tổng tiền",
                    Text = ViewModel.SelectedPurchaseOrder.TotalAmount.ToString()
                };

                stackPanel.Children.Add(customerIDTextBox);
                stackPanel.Children.Add(orderDateDatePicker);
                stackPanel.Children.Add(totalAmountTextBox);

                dialog.Content = stackPanel;

                dialog.PrimaryButtonClick += async (s, args) =>
                {
                    ViewModel.SelectedPurchaseOrder.SupplierID = int.Parse(customerIDTextBox.Text);
                    ViewModel.SelectedPurchaseOrder.OrderDate = orderDateDatePicker.Date.DateTime;
                    ViewModel.SelectedPurchaseOrder.TotalAmount = decimal.Parse(totalAmountTextBox.Text);

                    await ViewModel.UpdatePurchaseOrder(ViewModel.SelectedPurchaseOrder);
                };

                await dialog.ShowAsync();
            }
        }

        //private async void AddPurchaseOrderDetail_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new ContentDialog
        //    {
        //        Title = "Thêm chi tiết đơn hàng mới",
        //        PrimaryButtonText = "Thêm",
        //        CloseButtonText = "Hủy",
        //        DefaultButton = ContentDialogButton.Primary,
        //        XamlRoot = this.XamlRoot,
        //        MinWidth = 400
        //    };

        //    var stackPanel = new StackPanel();

        //    var purchaseOrderIDTextBox = new TextBox
        //    {
        //        Header = "Mã đơn hàng",
        //        PlaceholderText = "Nhập mã đơn hàng"
        //    };

        //    var productIDTextBox = new TextBox
        //    {
        //        Header = "Mã sản phẩm",
        //        PlaceholderText = "Nhập mã sản phẩm"
        //    };

        //    var quantityTextBox = new TextBox
        //    {
        //        Header = "Số lượng",
        //        PlaceholderText = "Nhập số lượng"
        //    };

        //    var priceTextBox = new TextBox
        //    {
        //        Header = "Đơn giá",
        //        PlaceholderText = "Nhập đơn giá"
        //    };

        //    stackPanel.Children.Add(purchaseOrderIDTextBox);
        //    stackPanel.Children.Add(productIDTextBox);
        //    stackPanel.Children.Add(quantityTextBox);
        //    stackPanel.Children.Add(priceTextBox);

        //    dialog.Content = stackPanel;

        //    dialog.PrimaryButtonClick += async (s, args) =>
        //    {
        //        var newPurchaseOrderDetail = new PurchaseOrderDetail
        //        {
        //            PurchaseOrderID = int.Parse(purchaseOrderIDTextBox.Text),
        //            ProductID = int.Parse(productIDTextBox.Text),
        //            Quantity = int.Parse(quantityTextBox.Text),
        //            UnitPrice = decimal.Parse(priceTextBox.Text)
        //        };

        //        await ViewModel.AddPurchaseOrderDetail(newPurchaseOrderDetail);
        //    };

        //    await dialog.ShowAsync();
        //}
    }
}