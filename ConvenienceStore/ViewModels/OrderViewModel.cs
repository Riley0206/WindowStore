using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConvenienceStore.Models;
using ConvenienceStore.Services;

namespace ConvenienceStore.ViewModels
{
    public partial class OrderViewModel : ObservableObject
    {
        #region Private Fields
        private readonly OrderDatabaseService _databaseService;
        private int _pageSize = 10;
        private int _currentPage = 1;
        private int _totalPages;
        private ObservableCollection<Order> _allOrders;
        private ObservableCollection<Order> _displayedOrders;
        private ObservableCollection<OrderDetail> _selectedOrderDetails;
        private Order _selectedOrder;
        #endregion

        #region Public Properties
        public ObservableCollection<Order> AllOrders
        {
            get => _allOrders;
            set => SetProperty(ref _allOrders, value);
        }

        public ObservableCollection<Order> DisplayedOrders
        {
            get => _displayedOrders;
            set => SetProperty(ref _displayedOrders, value);
        }

        public ObservableCollection<OrderDetail> SelectedOrderDetails
        {
            get => _selectedOrderDetails;
            set => SetProperty(ref _selectedOrderDetails, value);
        }

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (SetProperty(ref _pageSize, value))
                {
                    CalculateTotalPages();
                }
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    UpdateDisplayedOrders();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        #endregion

        #region Constructor
        public OrderViewModel(OrderDatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _allOrders = new ObservableCollection<Order>();
            _displayedOrders = new ObservableCollection<Order>();
            _selectedOrderDetails = new ObservableCollection<OrderDetail>();
        }
        #endregion

        #region Commands
        [RelayCommand]
        public async Task AddOrder(Order newOrder)
        {
            if (newOrder == null) return;
            await _databaseService.AddOrderAsync(newOrder);
            await LoadData();
        }

        [RelayCommand]
        public async Task DeleteOrder()
        {
            if (SelectedOrder == null) return;
            try
            {
                await _databaseService.DeleteOrderAsync(SelectedOrder.OrderID);
                await LoadData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting order: {ex.Message}");
                throw;
            }
        }

        [RelayCommand]
        public async Task UpdateOrder(Order updatedOrder)
        {
            await _databaseService.UpdateOrderAsync(updatedOrder);
            CalculateTotalPages();
            UpdateDisplayedOrders();
        }
        #endregion

        #region Methods
        public async Task LoadData()
        {
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            var Orders = await _databaseService.GetOrdersAsync();
            AllOrders = new ObservableCollection<Order>(Orders);
            CalculateTotalPages();
            UpdateDisplayedOrders();
        }

        private async Task LoadOrderDetails(int OrderId)
        {
            var OrderDetails = await _databaseService.GetOrderDetailAsync(OrderId);
            //SelectedOrderDetails = new ObservableCollection<OrderDetail>(OrderDetails);
        }

        private void CalculateTotalPages()
        {
            TotalPages = (int)Math.Ceiling((double)AllOrders.Count / PageSize);
        }

        private void UpdateDisplayedOrders()
        {
            var startIndex = (CurrentPage - 1) * PageSize;
            DisplayedOrders.Clear();
            for (int i = startIndex; i < startIndex + PageSize && i < AllOrders.Count; i++)
            {
                DisplayedOrders.Add(AllOrders[i]);
            }
        }

        public void NextPage()
        {
            if (HasNextPage)
            {
                CurrentPage++;
            }
        }

        public void PreviousPage()
        {
            if (HasPreviousPage)
            {
                CurrentPage--;
            }
        }

        public void FilterOrdersByDate(DateTime startDate, DateTime endDate)
        {
            var filteredOrders = AllOrders.Where(po => po.OrderDate >= startDate && po.OrderDate <= endDate).ToList();
            AllOrders = new ObservableCollection<Order>(filteredOrders);
            CalculateTotalPages();
            UpdateDisplayedOrders();
        }

        public void LoadAllOrders()
        {
            LoadData().ConfigureAwait(false);
        }

        public async Task LoadSelectedOrderDetails()
        {
            if (SelectedOrder != null)
            {
                await LoadOrderDetails(SelectedOrder.OrderID);
            }
        }
        #endregion
    }
}