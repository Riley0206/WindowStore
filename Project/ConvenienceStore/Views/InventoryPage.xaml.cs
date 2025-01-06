using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ConvenienceStore.ViewModels;
using Microsoft.UI.Xaml;
using System;
using ConvenienceStore.Models;
using ConvenienceStore.Services;
using System.Diagnostics;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using System.Linq;

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
        // Event handler cho nut danh muc san pham
        private void LoadAllProducts_TextBlockPressed(object sender, PointerRoutedEventArgs e)
        {
            ViewModel.LoadAllProducts();
        }

        // Event handler cho nút thêm sản phẩm mới
        public async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Thêm sản phẩm mới",
                PrimaryButtonText = "Thêm",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot,
            };

            // Tạo Grid và các cột
            var grid = new Grid { Margin = new Thickness(10) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Tạo các TextBox và ComboBox
            var productNameLabel = new TextBlock { Text = "Tên sản phẩm:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
            var productNameBox = new TextBox { Margin = new Thickness(0, 5, 0, 5) };

            var brandLabel = new TextBlock { Text = "Thương hiệu:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
            var brandBox = new TextBox { Margin = new Thickness(0, 5, 0, 5) };

            var quantityLabel = new TextBlock { Text = "Số lượng:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
            var quantityBox = new NumberBox { Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact, Margin = new Thickness(0, 5, 0, 5) };

            var priceLabel = new TextBlock { Text = "Giá bán:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
            var priceBox = new NumberBox { Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact, Margin = new Thickness(0, 5, 0, 5) };

            var costPriceLabel = new TextBlock { Text = "Giá vốn:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
            var costPriceBox = new NumberBox { Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact, Margin = new Thickness(0, 5, 0, 5) };

            var unitLabel = new TextBlock { Text = "Đơn vị tính:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
            var unitComboBox = new ComboBox
            {
                ItemsSource = new string[] { "Lon", "Hộp", "Cái", "Cây", "Chai", "Kg", "Gói", "Bịch" },
                Margin = new Thickness(0, 5, 0, 5),

            };
            var categoryLabel = new TextBlock { Text = "Danh mục:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
            var categoryComboBox = new ComboBox
            {
                ItemsSource = ViewModel.Categories,
                DisplayMemberPath = "CategoryName",
                Margin = new Thickness(0, 5, 0, 5),

            };
            // Thêm các control vào grid
            grid.Children.Add(productNameLabel);
            Grid.SetColumn(productNameLabel, 0);
            Grid.SetRow(productNameLabel, 0);
            grid.Children.Add(productNameBox);
            Grid.SetColumn(productNameBox, 1);
            Grid.SetRow(productNameBox, 0);

            grid.Children.Add(brandLabel);
            Grid.SetColumn(brandLabel, 0);
            Grid.SetRow(brandLabel, 1);
            grid.Children.Add(brandBox);
            Grid.SetColumn(brandBox, 1);
            Grid.SetRow(brandBox, 1);

            grid.Children.Add(quantityLabel);
            Grid.SetColumn(quantityLabel, 0);
            Grid.SetRow(quantityLabel, 2);
            grid.Children.Add(quantityBox);
            Grid.SetColumn(quantityBox, 1);
            Grid.SetRow(quantityBox, 2);

            grid.Children.Add(priceLabel);
            Grid.SetColumn(priceLabel, 0);
            Grid.SetRow(priceLabel, 3);
            grid.Children.Add(priceBox);
            Grid.SetColumn(priceBox, 1);
            Grid.SetRow(priceBox, 3);

            grid.Children.Add(costPriceLabel);
            Grid.SetColumn(costPriceLabel, 0);
            Grid.SetRow(costPriceLabel, 4);
            grid.Children.Add(costPriceBox);
            Grid.SetColumn(costPriceBox, 1);
            Grid.SetRow(costPriceBox, 4);

            grid.Children.Add(unitLabel);
            Grid.SetColumn(unitLabel, 0);
            Grid.SetRow(unitLabel, 5);
            grid.Children.Add(unitComboBox);
            Grid.SetColumn(unitComboBox, 1);
            Grid.SetRow(unitComboBox, 5);

            grid.Children.Add(categoryLabel);
            Grid.SetColumn(categoryLabel, 0);
            Grid.SetRow(categoryLabel, 6);
            grid.Children.Add(categoryComboBox);
            Grid.SetColumn(categoryComboBox, 1);
            Grid.SetRow(categoryComboBox, 6);

            // Đặt Grid vào ScrollViewer
            var scrollViewer = new ScrollViewer
            {
                Content = grid,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            dialog.Content = scrollViewer;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (categoryComboBox.SelectedItem == null || unitComboBox.SelectedItem == null)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Lỗi",
                        Content = "Vui lòng chọn danh mục và đơn vị sản phẩm",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot,
                    };
                    await errorDialog.ShowAsync();
                    return;
                }
                var selectedCategory = categoryComboBox.SelectedItem as Category;
                if (selectedCategory == null)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Lỗi",
                        Content = "Vui lòng chọn một danh mục.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot,
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
                    Unit = unitComboBox.SelectedItem.ToString(),
                    CategoryID = selectedCategory.CategoryID
                };

                await ViewModel.AddProduct(newProduct);
            }
        }

        // Event handler cho việc cập nhật số lượng
        public async void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is Product product)
            {
                var dialog = new ContentDialog
                {
                    Title = $"Cập nhật sản phẩm: {product.ProductName}",
                    PrimaryButtonText = "Cập nhật",
                    CloseButtonText = "Hủy",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot,

                };

                var grid = new Grid { Margin = new Thickness(10) };
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                // Tạo các TextBox và ComboBox
                var productNameLabel = new TextBlock { Text = "Tên sản phẩm:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
                var productNameBox = new TextBox { Margin = new Thickness(0, 5, 0, 5), Text = product.ProductName };

                var brandLabel = new TextBlock { Text = "Thương hiệu:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
                var brandBox = new TextBox { Margin = new Thickness(0, 5, 0, 5), Text = product.Brand };

                var quantityLabel = new TextBlock { Text = "Số lượng:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
                var quantityBox = new NumberBox { Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact, Margin = new Thickness(0, 5, 0, 5), Value = product.QuantityInStock };

                var priceLabel = new TextBlock { Text = "Giá bán:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
                var priceBox = new NumberBox { Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact, Margin = new Thickness(0, 5, 0, 5), Value = (double)product.Price };

                var costPriceLabel = new TextBlock { Text = "Giá vốn:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
                var costPriceBox = new NumberBox { Minimum = 0, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact, Margin = new Thickness(0, 5, 0, 5), Value = (double)product.CostPrice };

                var unitLabel = new TextBlock { Text = "Đơn vị tính:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
                var unitComboBox = new ComboBox
                {
                    ItemsSource = new string[] { "Lon", "Hộp", "Cái", "Cây", "Chai", "Kg", "Gói", "Bịch" },
                    Margin = new Thickness(0, 5, 0, 5),
                    SelectedItem = product.Unit,

                };
                var categoryLabel = new TextBlock { Text = "Danh mục:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 10, 5) };
                var categoryComboBox = new ComboBox
                {
                    ItemsSource = ViewModel.Categories,
                    DisplayMemberPath = "CategoryName",
                    Margin = new Thickness(0, 5, 0, 5),

                };
                if (product != null)
                {
                    categoryComboBox.SelectedItem = ViewModel.Categories.FirstOrDefault(c => c.CategoryID == product.CategoryID);
                }

                // Thêm các control vào grid
                grid.Children.Add(productNameLabel);
                Grid.SetColumn(productNameLabel, 0);
                Grid.SetRow(productNameLabel, 0);
                grid.Children.Add(productNameBox);
                Grid.SetColumn(productNameBox, 1);
                Grid.SetRow(productNameBox, 0);

                grid.Children.Add(brandLabel);
                Grid.SetColumn(brandLabel, 0);
                Grid.SetRow(brandLabel, 1);
                grid.Children.Add(brandBox);
                Grid.SetColumn(brandBox, 1);
                Grid.SetRow(brandBox, 1);

                grid.Children.Add(quantityLabel);
                Grid.SetColumn(quantityLabel, 0);
                Grid.SetRow(quantityLabel, 2);
                grid.Children.Add(quantityBox);
                Grid.SetColumn(quantityBox, 1);
                Grid.SetRow(quantityBox, 2);

                grid.Children.Add(priceLabel);
                Grid.SetColumn(priceLabel, 0);
                Grid.SetRow(priceLabel, 3);
                grid.Children.Add(priceBox);
                Grid.SetColumn(priceBox, 1);
                Grid.SetRow(priceBox, 3);

                grid.Children.Add(costPriceLabel);
                Grid.SetColumn(costPriceLabel, 0);
                Grid.SetRow(costPriceLabel, 4);
                grid.Children.Add(costPriceBox);
                Grid.SetColumn(costPriceBox, 1);
                Grid.SetRow(costPriceBox, 4);

                grid.Children.Add(unitLabel);
                Grid.SetColumn(unitLabel, 0);
                Grid.SetRow(unitLabel, 5);
                grid.Children.Add(unitComboBox);
                Grid.SetColumn(unitComboBox, 1);
                Grid.SetRow(unitComboBox, 5);

                grid.Children.Add(categoryLabel);
                Grid.SetColumn(categoryLabel, 0);
                Grid.SetRow(categoryLabel, 6);
                grid.Children.Add(categoryComboBox);
                Grid.SetColumn(categoryComboBox, 1);
                Grid.SetRow(categoryComboBox, 6);
                // Đặt Grid vào ScrollViewer
                var scrollViewer = new ScrollViewer
                {
                    Content = grid,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
                };
                dialog.Content = scrollViewer;
                if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    if (categoryComboBox.SelectedItem == null || unitComboBox.SelectedItem == null)
                    {
                        var errorDialog = new ContentDialog
                        {
                            Title = "Lỗi",
                            Content = "Vui lòng chọn danh mục và đơn vị sản phẩm",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot,

                        };
                        await errorDialog.ShowAsync();
                        return;
                    }
                    var selectedCategory = categoryComboBox.SelectedItem as Category;
                    if (selectedCategory == null)
                    {
                        var errorDialog = new ContentDialog
                        {
                            Title = "Lỗi",
                            Content = "Vui lòng chọn một danh mục.",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot,

                        };
                        await errorDialog.ShowAsync();
                        return;
                    }

                    product.ProductName = productNameBox.Text;
                    product.Brand = brandBox.Text;
                    product.QuantityInStock = (int)quantityBox.Value;
                    product.Price = (decimal)priceBox.Value;
                    product.CostPrice = (decimal)costPriceBox.Value;
                    product.Unit = unitComboBox.SelectedItem.ToString();
                    product.CategoryID = selectedCategory.CategoryID;


                    await ViewModel.UpdateProduct(product);
                }
            }
        }


        // Event handler cho việc xóa 1 dòng sản phẩm
        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is Product product)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Xác nhận xóa",
                    Content = $"Bạn có chắc chắn muốn xóa sản phẩm '{product.ProductName}'?",
                    PrimaryButtonText = "Xóa",
                    CloseButtonText = "Hủy",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot,

                };

                if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    try
                    {
                        ViewModel.SelectedProduct = product;
                        await ViewModel.DeleteProduct();
                    }
                    catch (Exception ex)
                    {
                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Lỗi",
                            Content = $"Không thể xóa sản phẩm: {ex.Message}",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot,

                        };
                        await errorDialog.ShowAsync();
                    }
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