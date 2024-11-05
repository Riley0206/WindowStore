using ConvenienceStore.Models;
using ConvenienceStore.Services;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ConvenienceStore.ViewModels
{
    public partial class InventoryViewModel : ObservableObject
    {
        #region Private Fields
        private readonly DatabaseService _databaseService;
        private bool _categoriesLoaded;
        private int _pageSize = 10;
        private int _currentPage = 1;
        private int _totalPages;
        private ObservableCollection<Product> _allProducts;
        private ObservableCollection<Product> _displayedProducts;
        private ObservableCollection<Category> _categories;
        private Product _selectedProduct;
        #endregion

        #region Public Properties
        public ObservableCollection<Product> AllProducts
        {
            get => _allProducts;
            set => SetProperty(ref _allProducts, value);
        }

        public ObservableCollection<Product> DisplayedProducts
        {
            get => _displayedProducts;
            set => SetProperty(ref _displayedProducts, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (SetProperty(ref _pageSize, value))
                {
                    CalculateTotalPages();
                    UpdateDisplayedProducts();
                }
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (value < 1) value = 1;
                if (value > TotalPages) value = TotalPages;
                if (SetProperty(ref _currentPage, value))
                {
                    UpdateDisplayedProducts();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            private set => SetProperty(ref _totalPages, value);
        }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        #endregion

        #region Constructor
        public InventoryViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _allProducts = new ObservableCollection<Product>();
            _displayedProducts = new ObservableCollection<Product>();
            _categories = new ObservableCollection<Category>();

            _ = LoadData();
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task AddProduct(Product newProduct)
        {
            if (newProduct == null) return;
            await _databaseService.AddProductAsync(newProduct);
            await LoadData();
        }

        [RelayCommand]
        private async Task UpdateQuantity(int newQuantity)
        {
            if (SelectedProduct == null) return;
            await _databaseService.UpdateProductQuantityAsync(SelectedProduct.ProductID, newQuantity);
            await LoadData();
        }

        [RelayCommand]
        public async Task DeleteProduct()
        {
            if (SelectedProduct == null) return;
            try
            {
                await _databaseService.DeleteProductAsync(SelectedProduct.ProductID);
                await LoadData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting product: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Data Loading Methods
        public async Task LoadData()
        {
            try
            {
                var productsData = await _databaseService.GetProductsAsync();
                var categoriesData = await _databaseService.GetCategoriesAsync();

                AllProducts = new ObservableCollection<Product>(productsData);
                Categories = new ObservableCollection<Category>(categoriesData);

                LoadAllProducts();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                throw;
            }
        }

        public async Task LoadCategoriesAsync()
        {
            if (!_categoriesLoaded)
            {
                var categoriesData = await _databaseService.GetCategoriesAsync();
                Categories = new ObservableCollection<Category>(categoriesData);
                _categoriesLoaded = true;
            }
        }
        #endregion

        #region Filtering Methods
        public void FilterProductsByCategory(int categoryId)
        {
            // Sử dụng AllProducts làm nguồn gốc và chỉ lọc DisplayedProducts
            var filteredProducts = AllProducts.Where(p => p.CategoryID == categoryId).ToList();
            DisplayedProducts = new ObservableCollection<Product>(filteredProducts);

            // Cập nhật số trang và hiển thị trang đầu tiên
            CalculateTotalPages();
            CurrentPage = 1;
            UpdateDisplayedProducts();
        }

        public void LoadAllProducts()
        {
            DisplayedProducts = new ObservableCollection<Product>(AllProducts);

            // Cập nhật số trang và hiển thị trang đầu tiên
            CalculateTotalPages();
            CurrentPage = 1;
            UpdateDisplayedProducts();
        }
        #endregion

        #region Pagination Methods
        private void CalculateTotalPages()
        {
            TotalPages = (AllProducts?.Count ?? 0) == 0 ? 1 : (int)Math.Ceiling(AllProducts.Count / (double)PageSize);
            OnPropertyChanged(nameof(HasPreviousPage));
            OnPropertyChanged(nameof(HasNextPage));
        }

        private void UpdateDisplayedProducts()
        {
            if (AllProducts == null || !AllProducts.Any())
            {
                DisplayedProducts = new ObservableCollection<Product>();
                return;
            }

            var items = DisplayedProducts
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            DisplayedProducts = new ObservableCollection<Product>(items);
        }

        [RelayCommand]
        public void NextPage()
        {
            if (HasNextPage)
            {
                CurrentPage++;
            }
        }

        [RelayCommand]
        public void PreviousPage()
        {
            if (HasPreviousPage)
            {
                CurrentPage--;
            }
        }
        #endregion
    }
}