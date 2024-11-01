using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ConvenienceStore.Models;
using ConvenienceStore.Services;
using ConvenienceStore.Helpers;

namespace ConvenienceStore.ViewModels
{
    public class InventoryViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseHelper _databaseHelper;
        private readonly InventoryService _inventoryService;
        public ObservableCollection<ProductModel> InventoryList { get; set; } = new ObservableCollection<ProductModel>();
        public ICommand LoadInventoryCommand { get; }
        public ICommand OrderProductCommand { get; }

        public InventoryViewModel()
        {
            string connectionString = "Data Source=.\\SQL22;Initial Catalog=ConvenienceStoreDB;Integrated Security=True;Trust Server Certificate=True";
            _databaseHelper = new DatabaseHelper(connectionString);
            _inventoryService = new InventoryService(_databaseHelper);

            LoadInventoryCommand = new RelayCommand<object>(_ => LoadInventory());
            OrderProductCommand = new RelayCommand<int>(OrderProduct);

            LoadInventoryCommand.Execute(null); 
        }

        private async Task LoadInventory()
        {
            try
            {
                var products = await _inventoryService.GetInventory();
                InventoryList.Clear();
                foreach (var product in products)
                    InventoryList.Add(product);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi: có thể là ghi log hoặc hiển thị thông báo
            }
        }

        private void OrderProduct(int productId)
        {
            try
            {
                _inventoryService.PlaceOrder(productId);
                LoadInventoryCommand.Execute(null);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi: hiển thị thông báo hoặc ghi log
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
