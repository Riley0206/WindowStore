using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ConvenienceStore.ViewModels;
using Microsoft.UI.Xaml;
using System;
using ConvenienceStore.Models;
using ConvenienceStore.Services;

namespace ConvenienceStore.Views
{
    public sealed partial class InventoryPage : Page
    {
        public InventoryViewModel ViewModel { get; }

        public InventoryPage()
        {
            // Khởi tạo ViewModel với DatabaseService
            string connectionString = "Data Source=.\\SQL22;Initial Catalog=ConvenienceStoreDB;Integrated Security=True;Trust Server Certificate=True";
            var databaseService = new DatabaseService(connectionString);
            ViewModel = new InventoryViewModel(databaseService);

            this.InitializeComponent();

            // Đăng ký các event handlers
            this.Loaded += InventoryPage_Loaded;
        }

        private void InventoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Có thể thêm các khởi tạo bổ sung khi page được load
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

        // Event handler cho nút thêm sản phẩm mới
        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Hiển thị dialog thêm sản phẩm mới
                var dialog = new ContentDialog()
                {
                    Title = "Thêm sản phẩm mới",
                    PrimaryButtonText = "Thêm",
                    CloseButtonText = "Hủy",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    await ViewModel.AddProductCommand.ExecuteAsync(null);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và hiển thị thông báo cho người dùng
                ContentDialog errorDialog = new ContentDialog()
                {
                    Title = "Lỗi",
                    Content = $"Không thể thêm sản phẩm: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }

        // Event handler cho việc cập nhật số lượng
        private async void UpdateQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedProduct == null)
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Thông báo",
                    Content = "Vui lòng chọn sản phẩm cần cập nhật",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            try
            {
                var quantityDialog = new ContentDialog()
                {
                    Title = $"Cập nhật số lượng cho {ViewModel.SelectedProduct.ProductName}",
                    PrimaryButtonText = "Cập nhật",
                    CloseButtonText = "Hủy",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };

                // Thêm TextBox cho người dùng nhập số lượng mới
                var quantityBox = new NumberBox()
                {
                    Header = "Số lượng mới:",
                    Value = ViewModel.SelectedProduct.QuantityInStock,
                    Minimum = 0,
                    SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact
                };

                quantityDialog.Content = quantityBox;

                var result = await quantityDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    await ViewModel.UpdateQuantityCommand.ExecuteAsync((int)quantityBox.Value);
                }
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog()
                {
                    Title = "Lỗi",
                    Content = $"Không thể cập nhật số lượng: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }

        // Event handler cho việc lọc sản phẩm theo danh mục
        private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is Category selectedCategory)
            {
                // Lọc sản phẩm theo danh mục được chọn
                //ViewModel.FilterProductsByCategory(selectedCategory.CategoryID);
            }
        }

        // Event handler cho DataGrid row double click
        private async void ProductsDataGrid_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (ViewModel.SelectedProduct != null)
            {
                try
                {
                    // Hiển thị dialog chi tiết sản phẩm
                    var detailsDialog = new ContentDialog()
                    {
                        Title = "Chi tiết sản phẩm",
                        PrimaryButtonText = "Đóng",
                        XamlRoot = this.XamlRoot
                    };

                    var product = ViewModel.SelectedProduct;
                    var content = new StackPanel() { Spacing = 10 };
                    content.Children.Add(new TextBlock() { Text = $"Mã sản phẩm: {product.ProductID}" });
                    content.Children.Add(new TextBlock() { Text = $"Tên sản phẩm: {product.ProductName}" });
                    content.Children.Add(new TextBlock() { Text = $"Thương hiệu: {product.Brand}" });
                    content.Children.Add(new TextBlock() { Text = $"Số lượng tồn: {product.QuantityInStock}" });
                    content.Children.Add(new TextBlock() { Text = $"Mức tồn kho tối thiểu: {product.ReorderLevel}" });
                    content.Children.Add(new TextBlock() { Text = $"Giá: {product.Price:C}" });
                    content.Children.Add(new TextBlock() { Text = $"Danh mục: {product.Category.CategoryName}" });

                    detailsDialog.Content = content;
                    await detailsDialog.ShowAsync();
                }
                catch (Exception ex)
                {
                    ContentDialog errorDialog = new ContentDialog()
                    {
                        Title = "Lỗi",
                        Content = $"Không thể hiển thị chi tiết sản phẩm: {ex.Message}",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }
    }
}