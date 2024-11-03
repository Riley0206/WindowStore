using ConvenienceStore.Models;
using ConvenienceStore.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace ConvenienceStore.ViewModels
{
    public partial class InventoryViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Product> products;

        [ObservableProperty]
        private ObservableCollection<Category> categories;

        [ObservableProperty]
        private Product selectedProduct;

        private ObservableCollection<Product> allProducts;
        public ObservableCollection<Product> AllProducts
        {
            get => allProducts;
            set => SetProperty(ref allProducts, value);
        }

        public InventoryViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            Products = new ObservableCollection<Product>();
            Categories = new ObservableCollection<Category>();
        }


        // Command to add product
        [RelayCommand]
        private async Task AddProduct()
        {
            if (SelectedProduct == null) return;
            await _databaseService.AddProductAsync(SelectedProduct);
            await LoadData();
        }

        // Command to update product quantity
        [RelayCommand]
        private async Task UpdateQuantity(int newQuantity)
        {
            if (SelectedProduct == null) return;
            await _databaseService.UpdateProductQuantityAsync(SelectedProduct.ProductID, newQuantity);
            await LoadData();
        }

        // Load products and categories from database
        public async Task LoadData()
        {
            try
            {
                Debug.WriteLine("Starting to load data");

                var productsData = await _databaseService.GetProductsAsync();
                var categoriesData = await _databaseService.GetCategoriesAsync();

                Debug.WriteLine($"Retrieved {productsData.Count} products and {categoriesData.Count} categories from database");
                
                AllProducts = new ObservableCollection<Product>(productsData);
                Products = new ObservableCollection<Product>(productsData);
                Categories = new ObservableCollection<Category>(categoriesData);

                Debug.WriteLine("Data loaded successfully");
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Error: {ex.Message}");
                Debug.WriteLine($"Error Number: {ex.Number}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public void FilterProductsByCategory(int categoryId)
        {
            var filteredProducts = AllProducts.Where(p => p.CategoryID == categoryId).ToList();
            Products = new ObservableCollection<Product>(filteredProducts);
        }

        public void LoadAllProducts()
        {
            Products = new ObservableCollection<Product>(AllProducts);
        }

    }
}
