using ConvenienceStore.Models;
using ConvenienceStore.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ConvenienceStore.ViewModels
{
    public partial class OrderViewModel : ObservableObject
    {
        #region Private Fields
        private readonly OrderDatabaseService _databaseService; // Changed to OrderDatabaseService
        private int _pageSize = 10;
        private int _currentPage = 1;
        private int _totalPages;
        private ObservableCollection<Order> _allOrders;
        private ObservableCollection<Order> _filteredOrders;
        private ObservableCollection<Order> _displayedOrders;

        private ObservableCollection<PurchaseOrder> _allPurchaseOrders;
        private ObservableCollection<PurchaseOrder> _filteredPurchaseOrders;
        private ObservableCollection<PurchaseOrder> _displayedPurchaseOrders;

        private ObservableCollection<Transaction> _transactions;
        private Order _selectedOrder;
        private PurchaseOrder _selectedPurchaseOrder;
        private DateTime? _startDateFilter;
        private DateTime? _endDateFilter;
        private ObservableCollection<OrderDetail> _selectedOrderDetails;
        private ObservableCollection<PurchaseOrderDetail> _selectedPurchaseOrderDetails;
        private decimal _totalSales;
        private decimal _totalPurchases;
        private int _purchaseOrderCurrentPage = 1;
        private int _purchaseOrderTotalPages;
        #endregion

        #region Public Properties
        public ObservableCollection<Order> AllOrders
        {
            get => _allOrders;
            set => SetProperty(ref _allOrders, value);
        }

        public ObservableCollection<Order> FilteredOrders
        {
            get => _filteredOrders;
            set => SetProperty(ref _filteredOrders, value);
        }

        public ObservableCollection<Order> DisplayedOrders
        {
            get => _displayedOrders;
            set => SetProperty(ref _displayedOrders, value);
        }
        public ObservableCollection<PurchaseOrder> AllPurchaseOrders
        {
            get => _allPurchaseOrders;
            set => SetProperty(ref _allPurchaseOrders, value);
        }
        public ObservableCollection<PurchaseOrder> FilteredPurchaseOrders
        {
            get => _filteredPurchaseOrders;
            set => SetProperty(ref _filteredPurchaseOrders, value);
        }
        public ObservableCollection<PurchaseOrder> DisplayedPurchaseOrders
        {
            get => _displayedPurchaseOrders;
            set => SetProperty(ref _displayedPurchaseOrders, value);
        }
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                Debug.WriteLine($"SelectedOrder changed to: {value?.OrderID}");
                if (SetProperty(ref _selectedOrder, value))
                {
                    _ = LoadSelectedOrderDetails();
                }
            }
        }
        public PurchaseOrder SelectedPurchaseOrder
        {
            get => _selectedPurchaseOrder;
            set
            {
                Debug.WriteLine($"SelectedPurchaseOrder changed to: {value?.PurchaseOrderID}");
                if (SetProperty(ref _selectedPurchaseOrder, value))
                {
                    _ = LoadSelectedPurchaseOrderDetails();
                }
            }
        }
        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set => SetProperty(ref _transactions, value);
        }
        public ObservableCollection<OrderDetail> SelectedOrderDetails
        {
            get => _selectedOrderDetails;
            set => SetProperty(ref _selectedOrderDetails, value);
        }
        public ObservableCollection<PurchaseOrderDetail> SelectedPurchaseOrderDetails
        {
            get => _selectedPurchaseOrderDetails;
            set => SetProperty(ref _selectedPurchaseOrderDetails, value);
        }
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (SetProperty(ref _pageSize, value))
                {
                    CurrentPage = 1;
                    PurchaseOrderCurrentPage = 1;
                    CalculateTotalPages();
                    CalculateTotalPurchaseOrderPages();
                    UpdateDisplayedOrders();
                    UpdateDisplayedPurchaseOrders();
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
                    UpdateDisplayedOrders();
                    OnPropertyChanged(nameof(HasPreviousPage));
                    OnPropertyChanged(nameof(HasNextPage));
                }
            }
        }

        public int PurchaseOrderCurrentPage
        {
            get => _purchaseOrderCurrentPage;
            set
            {
                if (value < 1) value = 1;
                if (value > PurchaseOrderTotalPages) value = PurchaseOrderTotalPages;

                if (SetProperty(ref _purchaseOrderCurrentPage, value))
                {
                    UpdateDisplayedPurchaseOrders();
                    OnPropertyChanged(nameof(HasPreviousPurchaseOrderPage));
                    OnPropertyChanged(nameof(HasNextPurchaseOrderPage));
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            private set => SetProperty(ref _totalPages, value);
        }

        public int PurchaseOrderTotalPages
        {
            get => _purchaseOrderTotalPages;
            private set => SetProperty(ref _purchaseOrderTotalPages, value);
        }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public bool HasPreviousPurchaseOrderPage => PurchaseOrderCurrentPage > 1;
        public bool HasNextPurchaseOrderPage => PurchaseOrderCurrentPage < PurchaseOrderTotalPages;
        public DateTime? StartDateFilter
        {
            get => _startDateFilter;
            set
            {
                SetProperty(ref _startDateFilter, value);
                ApplyFilters();
            }
        }
        public DateTime? EndDateFilter
        {
            get => _endDateFilter;
            set
            {
                SetProperty(ref _endDateFilter, value);
                ApplyFilters();
            }
        }
        public decimal TotalSales
        {
            get => _totalSales;
            set => SetProperty(ref _totalSales, value);
        }

        public decimal TotalPurchases
        {
            get => _totalPurchases;
            set => SetProperty(ref _totalPurchases, value);
        }
        #endregion

        #region Constructor
        public OrderViewModel(OrderDatabaseService databaseService) // Changed to OrderDatabaseService
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _allOrders = new ObservableCollection<Order>();
            _filteredOrders = new ObservableCollection<Order>();
            _displayedOrders = new ObservableCollection<Order>();
            _allPurchaseOrders = new ObservableCollection<PurchaseOrder>();
            _filteredPurchaseOrders = new ObservableCollection<PurchaseOrder>();
            _displayedPurchaseOrders = new ObservableCollection<PurchaseOrder>();
            _transactions = new ObservableCollection<Transaction>();
            _selectedOrderDetails = new ObservableCollection<OrderDetail>();
            _selectedPurchaseOrderDetails = new ObservableCollection<PurchaseOrderDetail>();
            _pageSize = 10;
            _currentPage = 1;
            _purchaseOrderCurrentPage = 1;

            _ = LoadData();
        }
        #endregion

        #region Commands
        [RelayCommand]
        public void NextPurchaseOrderPage()
        {
            if (HasNextPurchaseOrderPage)
            {
                PurchaseOrderCurrentPage++;
            }
        }

        [RelayCommand]
        public void PreviousPurchaseOrderPage()
        {
            if (HasPreviousPurchaseOrderPage)
            {
                PurchaseOrderCurrentPage--;
            }
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

        #region Data Loading Methods
        public async Task LoadData()
        {
            try
            {
                var ordersData = await _databaseService.GetOrdersAsync();
                AllOrders = new ObservableCollection<Order>(ordersData);
                FilteredOrders = new ObservableCollection<Order>(AllOrders);

                var purchaseOrdersData = await _databaseService.GetPurchaseOrdersAsync();
                AllPurchaseOrders = new ObservableCollection<PurchaseOrder>(purchaseOrdersData);
                FilteredPurchaseOrders = new ObservableCollection<PurchaseOrder>(AllPurchaseOrders);

                var transactionsData = await _databaseService.GetTransactionsAsync();
                Transactions = new ObservableCollection<Transaction>(transactionsData);

                CalculateTotalSales();
                CalculateTotalPurchases();
                CalculateTotalPages();
                CalculateTotalPurchaseOrderPages();
                CurrentPage = 1;
                PurchaseOrderCurrentPage = 1;
                UpdateDisplayedOrders();
                UpdateDisplayedPurchaseOrders();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                throw;
            }
        }

        private async Task LoadSelectedOrderDetails()
        {
            if (SelectedOrder != null)
            {
                var orderDetails = await _databaseService.GetOrderDetailsAsync(SelectedOrder.OrderID);
                SelectedOrderDetails = new ObservableCollection<OrderDetail>(orderDetails);
            }
            else
            {
                SelectedOrderDetails = new ObservableCollection<OrderDetail>();
            }

        }
        private async Task LoadSelectedPurchaseOrderDetails()
        {
            if (SelectedPurchaseOrder != null)
            {
                var purchaseOrderDetails = await _databaseService.GetPurchaseOrderDetailsAsync(SelectedPurchaseOrder.PurchaseOrderID);
                SelectedPurchaseOrderDetails = new ObservableCollection<PurchaseOrderDetail>(purchaseOrderDetails);
            }
            else
            {
                SelectedPurchaseOrderDetails = new ObservableCollection<PurchaseOrderDetail>();
            }
        }
        #endregion

        #region Filtering Methods
        private void ApplyFilters()
        {
            FilteredOrders = new ObservableCollection<Order>(AllOrders);
            FilteredPurchaseOrders = new ObservableCollection<PurchaseOrder>(AllPurchaseOrders);
            if (StartDateFilter.HasValue)
            {
                FilteredOrders = new ObservableCollection<Order>(FilteredOrders.Where(o => o.OrderDate >= StartDateFilter.Value).ToList());
                FilteredPurchaseOrders = new ObservableCollection<PurchaseOrder>(FilteredPurchaseOrders.Where(p => p.OrderDate >= StartDateFilter.Value).ToList());

            }

            if (EndDateFilter.HasValue)
            {
                FilteredOrders = new ObservableCollection<Order>(FilteredOrders.Where(o => o.OrderDate <= EndDateFilter.Value).ToList());
                FilteredPurchaseOrders = new ObservableCollection<PurchaseOrder>(FilteredPurchaseOrders.Where(p => p.OrderDate <= EndDateFilter.Value).ToList());
            }
            CalculateTotalPages();
            CalculateTotalPurchaseOrderPages();
            CurrentPage = 1;
            PurchaseOrderCurrentPage = 1;
            UpdateDisplayedOrders();
            UpdateDisplayedPurchaseOrders();
        }

        public void SearchOrdersByKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                FilteredOrders = new ObservableCollection<Order>(AllOrders);
            }
            else
            {
                var filteredOrders = AllOrders.Where(order =>
                    order.OrderID.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    order.Employee.EmployeeName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                     order.TotalAmount.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                     order.PaymentMethod.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                     order.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase)

                ).ToList();
                FilteredOrders = new ObservableCollection<Order>(filteredOrders);

            }
            CalculateTotalPages();
            CurrentPage = 1;
            UpdateDisplayedOrders();
        }
        public void SearchPurchaseOrdersByKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                FilteredPurchaseOrders = new ObservableCollection<PurchaseOrder>(AllPurchaseOrders);
            }
            else
            {
                var filteredPurchaseOrders = AllPurchaseOrders.Where(purchaseOrder =>
                     purchaseOrder.PurchaseOrderID.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    purchaseOrder.Supplier.SupplierName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                     purchaseOrder.Employee.EmployeeName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                     purchaseOrder.TotalAmount.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                     purchaseOrder.Status.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                      purchaseOrder.PaymentStatus.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                     purchaseOrder.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                ).ToList();
                FilteredPurchaseOrders = new ObservableCollection<PurchaseOrder>(filteredPurchaseOrders);
            }
            CalculateTotalPurchaseOrderPages();
            PurchaseOrderCurrentPage = 1;
            UpdateDisplayedPurchaseOrders();
        }

        #endregion

        #region Pagination Methods
        private void CalculateTotalPages()
        {
            var orderItemCount = FilteredOrders?.Count ?? 0;
            TotalPages = (orderItemCount + PageSize - 1) / PageSize;

            if (CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }
        private void CalculateTotalPurchaseOrderPages()
        {
            var purchaseOrderItemCount = FilteredPurchaseOrders?.Count ?? 0;
            PurchaseOrderTotalPages = (purchaseOrderItemCount + PageSize - 1) / PageSize;

            if (PurchaseOrderCurrentPage > PurchaseOrderTotalPages)
            {
                PurchaseOrderCurrentPage = PurchaseOrderTotalPages;
            }

        }
        private void UpdateDisplayedOrders()
        {
            if (FilteredOrders == null || FilteredOrders.Count == 0)
            {
                DisplayedOrders = new ObservableCollection<Order>();
                return;
            }

            var skip = (CurrentPage - 1) * PageSize;
            var items = FilteredOrders.Skip(skip).Take(PageSize).ToList();

            DisplayedOrders = new ObservableCollection<Order>(items);
        }
        private void UpdateDisplayedPurchaseOrders()
        {
            if (FilteredPurchaseOrders == null || FilteredPurchaseOrders.Count == 0)
            {
                DisplayedPurchaseOrders = new ObservableCollection<PurchaseOrder>();
                return;
            }

            var skip = (PurchaseOrderCurrentPage - 1) * PageSize;
            var items = FilteredPurchaseOrders.Skip(skip).Take(PageSize).ToList();

            DisplayedPurchaseOrders = new ObservableCollection<PurchaseOrder>(items);
        }

        #endregion

        #region Total Calculation Methods
        private void CalculateTotalSales()
        {
            TotalSales = AllOrders?.Sum(order => order.TotalAmount) ?? 0;
        }
        private void CalculateTotalPurchases()
        {
            TotalPurchases = AllPurchaseOrders?.Sum(purchaseOrder => purchaseOrder.TotalAmount) ?? 0;
        }
        #endregion
    }
}