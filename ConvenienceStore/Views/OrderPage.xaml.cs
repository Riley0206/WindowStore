using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
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
    public sealed partial class OrderPage : Page
    {
        public OrderViewModel ViewModel { get; }

        public OrderPage()
        {
            string connectionString = @"Data Source=DESKTOP-LD18TI4;Initial Catalog=ConvenienceStoreDB;Integrated Security=True;TrustServerCertificate=True";
            var databaseService = new OrderDatabaseService(connectionString);
            ViewModel = new OrderViewModel(databaseService);

            this.InitializeComponent();
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

        private void OrderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedOrder = OrderDataGrid.SelectedItem as Order;
        }

        private async void OrderDetails_Click(object sender, RoutedEventArgs e)
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

            var orderDetails = new ListView
            {
                ItemsSource = ViewModel.SelectedOrderDetails,
                ItemTemplate = (DataTemplate)Resources["OrderDetailTemplate"],
                SelectionMode = ListViewSelectionMode.None
            };

            stackPanel.Children.Add(orderDetails);
            dialog.Content = stackPanel;

            await dialog.ShowAsync();
        }

        private async void AddOrder_Click(object sender, RoutedEventArgs e)
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

            var orderDateBox = new DatePicker { Header = "Ngày đặt hàng" };
            var customerIDBox = new NumberBox { Header = "Mã khách hàng" };
            var totalAmountBox = new NumberBox { Header = "Tổng tiền", Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact };

            stackPanel.Children.Add(orderDateBox);
            stackPanel.Children.Add(customerIDBox);
            stackPanel.Children.Add(totalAmountBox);

            dialog.Content = stackPanel;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                var newOrder = new Order
                {
                    OrderDate = orderDateBox.Date.DateTime,
                    CustomerID = (int)customerIDBox.Value,
                    TotalAmount = (decimal)totalAmountBox.Value
                };

                try
                {
                    await ViewModel.AddOrder(newOrder);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error adding order: {ex.Message}");
                    Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }
        }

        private async void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Xác nhận xóa đơn hàng",
                Content = "Bạn có chắc chắn muốn xóa đơn hàng này?",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                try
                {
                    await ViewModel.DeleteOrder();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deleting order: {ex.Message}");
                    Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }
        }

        private async void UpdateOrder_Click(object sender, RoutedEventArgs e)
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

            var orderDateBox = new DatePicker { Header = "Ngày đặt hàng" };
            var customerIDBox = new NumberBox { Header = "Mã khách hàng" };
            var totalAmountBox = new NumberBox { Header = "Tổng tiền", Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact };

            stackPanel.Children.Add(orderDateBox);
            stackPanel.Children.Add(customerIDBox);
            stackPanel.Children.Add(totalAmountBox);

            dialog.Content = stackPanel;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                var updatedOrder = new Order
                {
                    OrderID = ViewModel.SelectedOrder.OrderID,
                    OrderDate = orderDateBox.Date.DateTime,
                    CustomerID = (int)customerIDBox.Value,
                    TotalAmount = (decimal)totalAmountBox.Value
                };

                try
                {
                    await ViewModel.UpdateOrder(updatedOrder);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error updating order: {ex.Message}");
                    Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentPage--;
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentPage++;
        }
    }
}