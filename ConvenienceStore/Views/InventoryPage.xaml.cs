using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ConvenienceStore.ViewModels;
using Microsoft.UI.Xaml;
using System;
using ConvenienceStore.Models;
using ConvenienceStore.Services;
using System.Diagnostics;

namespace ConvenienceStore.Views
{
    public sealed partial class InventoryPage : Page
    {
        public InventoryViewModel ViewModel { get; }

        public InventoryPage()
        {
            Debug.WriteLine("Initializing InventoryPage");

            // Khởi tạo ViewModel với DatabaseService
            string connectionString = @"Data Source=.\SQL22;Initial Catalog=ConvenienceStoreDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            var databaseService = new DatabaseService(connectionString);
            ViewModel = new InventoryViewModel(databaseService);

            this.InitializeComponent();

            // Đăng ký các event handlers
            this.Loaded += InventoryPage_Loaded;
        }

        private async void InventoryPage_Loaded(object sender, RoutedEventArgs e)
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

        // Event handler cho nút thêm sản phẩm mới
        // Event handler cho nút thêm sản phẩm mới
        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Thêm sản phẩm mới",
                PrimaryButtonText = "Thêm",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot,
                MinWidth = 400 // Đặt chiều rộng tối thiểu cho dialog
            };

            var stackPanel = new StackPanel();

            // Tạo các TextBox và ComboBox
            var productNameBox = new TextBox { Header = "Tên sản phẩm" };
            var brandBox = new TextBox { Header = "Thương hiệu" };
            var quantityBox = new NumberBox { Header = "Số lượng", Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact };
            var priceBox = new NumberBox { Header = "Giá bán", Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact };
            var costPriceBox = new NumberBox { Header = "Giá vốn", Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact };
            var unitBox = new TextBox { Header = "Đơn vị tính" };

            var categoryComboBox = new ComboBox
            {
                Header = "Danh mục sản phẩm",
                ItemsSource = ViewModel.Categories,
                DisplayMemberPath = "CategoryName",
                Width = 300 // Chiều rộng của ComboBox
            };

            // Thêm các thành phần vào StackPanel
            stackPanel.Children.Add(productNameBox);
            stackPanel.Children.Add(brandBox);
            stackPanel.Children.Add(quantityBox);
            stackPanel.Children.Add(priceBox);
            stackPanel.Children.Add(costPriceBox);
            stackPanel.Children.Add(unitBox);
            stackPanel.Children.Add(categoryComboBox);

            // Đặt StackPanel vào ScrollViewer
            var scrollViewer = new ScrollViewer
            {
                Content = stackPanel,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto // Hiện thanh cuộn khi cần thiết
            };

            dialog.Content = scrollViewer; // Đặt ScrollViewer làm nội dung của dialog

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (categoryComboBox.SelectedItem == null)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Lỗi",
                        Content = "Vui lòng chọn danh mục sản phẩm",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                    return;
                }

                var newProduct = new Product
                {
                    ProductName = productNameBox.Text,
                    Brand = brandBox.Text,
                    QuantityInStock = (int)quantityBox.Value,
                    Price = (decimal)priceBox.Value,
                    CostPrice = (decimal)costPriceBox.Value,
                    Unit = unitBox.Text,
                    CategoryID = ((Category)categoryComboBox.SelectedItem).CategoryID
                };

                await ViewModel.AddProductCommand.ExecuteAsync(newProduct);
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

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedProduct == null) return;

            ContentDialog dialog = new ContentDialog
            {
                Title = "Xác nhận xóa",
                Content = $"Bạn có chắc chắn muốn xóa sản phẩm '{ViewModel.SelectedProduct.ProductName}'?",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                try
                {
                    await ViewModel.DeleteProduct(); // Gọi trực tiếp phương thức thay vì qua Command
                }
                catch (Exception ex)
                {
                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Lỗi",
                        Content = $"Không thể xóa sản phẩm: {ex.Message}",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        // Event handler cho việc lọc sản phẩm theo danh mục
        private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is Category selectedCategory)
            {
                ViewModel.FilterProductsByCategory(selectedCategory.CategoryID);
            }
            else
            {
                ViewModel.LoadAllProducts();
            }
        }

        // Event handler cho DataGrid row double click
        private async void ProductsDataGrid_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (ViewModel.SelectedProduct != null)
            {
                try
                {
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
                    content.Children.Add(new TextBlock() { Text = $"Đơn vị tính: {product.Unit}" });
                    content.Children.Add(new TextBlock() { Text = $"Giá bán: {product.Price:C}" });
                    content.Children.Add(new TextBlock() { Text = $"Giá vốn: {product.CostPrice:C}" });
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