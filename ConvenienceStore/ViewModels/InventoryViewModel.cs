﻿using ConvenienceStore.Models;
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
        private ObservableCollection<Product> _filteredProducts;
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

        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set => SetProperty(ref _filteredProducts, value);
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
                    CurrentPage = 1;
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
                    OnPropertyChanged(nameof(HasPreviousPage));
                    OnPropertyChanged(nameof(HasNextPage));
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
            _filteredProducts = new ObservableCollection<Product>();
            _displayedProducts = new ObservableCollection<Product>();
            _categories = new ObservableCollection<Category>();

            // Khởi tạo giá trị mặc định
            _pageSize = 10;
            _currentPage = 1;

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

                // Khởi tạo FilteredProducts với tất cả sản phẩm
                FilteredProducts = new ObservableCollection<Product>(AllProducts);

                // Cập nhật phân trang
                CalculateTotalPages();
                CurrentPage = 1;
                UpdateDisplayedProducts();
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
            var filteredProducts = AllProducts.Where(p => p.CategoryID == categoryId).ToList();
            FilteredProducts = new ObservableCollection<Product>(filteredProducts);

            CalculateTotalPages();
            CurrentPage = 1;
            UpdateDisplayedProducts();
        }

        public void LoadAllProducts()
        {
            FilteredProducts = new ObservableCollection<Product>(AllProducts);

            CalculateTotalPages();
            CurrentPage = 1;
            UpdateDisplayedProducts();
        }

        public void SearchProductsByName(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                LoadAllProducts();
            }
            else
            {
                var filteredProducts = AllProducts
                    .Where(p => p.ProductName.Contains(productName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                FilteredProducts = new ObservableCollection<Product>(filteredProducts);

                CalculateTotalPages();
                CurrentPage = 1;
                UpdateDisplayedProducts();
            }
        }
        #endregion

        #region Pagination Methods
        private void CalculateTotalPages()
        {
            var itemCount = FilteredProducts?.Count ?? 0;
            TotalPages = itemCount == 0 ? 1 : (int)Math.Ceiling(itemCount / (double)PageSize);
        }

        private void UpdateDisplayedProducts()
        {
            if (FilteredProducts == null || !FilteredProducts.Any())
            {
                DisplayedProducts = new ObservableCollection<Product>();
                return;
            }

            var skip = (CurrentPage - 1) * PageSize;
            var items = FilteredProducts.Skip(skip).Take(PageSize).ToList();

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