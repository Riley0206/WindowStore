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
            string connectionString = @"Data Source=.\SQL22;Initial Catalog=ConvenienceStoreDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            var databaseService = new DatabaseService(connectionString);
            ViewModel = new InventoryViewModel(databaseService);

            this.InitializeComponent();
            this.Loaded += InventoryPage_Loaded;
        }

        private async void InventoryPage_Loaded(object sender, RoutedEventArgs e)
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
                MinWidth = 400
            };

            var stackPanel = new StackPanel();

            // Tạo các TextBox và ComboBox
            var productNameBox = new TextBox { Header = "Tên sản phẩm" };
            var brandBox = new TextBox { Header = "Thương hiệu" };
            var quantityBox = new NumberBox { Header = "Số lượng", Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact };
            var priceBox = new NumberBox { Header = "Giá bán", Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact };
            var costPriceBox = new NumberBox { Header = "Giá vốn", Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact };
            var unitBox = new TextBox
            {
                Header = "Đơn vị tính",
                Text = "VND",
                IsReadOnly = true
            };

            var categoryComboBox = new ComboBox
            {
                Header = "Danh mục sản phẩm",
                ItemsSource = ViewModel.Categories,
                DisplayMemberPath = "CategoryName",
                Width = 300
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
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            dialog.Content = scrollViewer;

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

                //TextBox cho người dùng nhập số lượng mới
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

        // Event handler cho việc xóa 1 dòng sản phẩm
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
                    await ViewModel.DeleteProduct();
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

        // Event handler cho nút Search
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Chỉ thực hiện tìm kiếm khi người dùng nhấn nút tìm kiếm
            string searchQuery = SearchBox.Text;
            ViewModel.SearchProductsByName(searchQuery);
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

        // Event handler cho nút Trang trước
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PreviousPage();
        }

        // Event handler cho nút Trang sau
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NextPage();
        }
    }
}