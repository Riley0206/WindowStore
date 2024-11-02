using ConvenienceStore.Models;
using ConvenienceStore.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

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

        public InventoryViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadData().ConfigureAwait(false);
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
        private async Task LoadData()
        {
            try
            {
                Products = new ObservableCollection<Product>(await _databaseService.GetProductsAsync());
                Categories = new ObservableCollection<Category>(await _databaseService.GetCategoriesAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }
    }
}
