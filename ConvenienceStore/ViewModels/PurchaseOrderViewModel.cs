using ConvenienceStore.Models;
using ConvenienceStore.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvenienceStore.Converters;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace ConvenienceStore.ViewModels
{
    public partial class PurchaseOrderViewModel
    {
        #region Private Fields
        private readonly PurchaseOrderDatabaseService _databaseService;
        private int _pageSize = 10;
        private int _currentPage = 1;
        private int _totalPages;
        private ObservableCollection<PurchaseOrder> _allPurchaseOrders;
        private ObservableCollection<PurchaseOrder> _displayedPurchaseOrders;
        private ObservableCollection<PurchaseOrderDetail> _selectedPurchaseOrderDetails;
        private PurchaseOrder _selectedPurchaseOrder;
        #endregion

        #region Public Properties
        public ObservableCollection<PurchaseOrder> AllPurchaseOrders
        {
            get => _allPurchaseOrders;
            set => _allPurchaseOrders = value;
        }

        public PurchaseOrder SelectedPurchaseOrder
        {
            get => _selectedPurchaseOrder;
            set => _selectedPurchaseOrder = value;
        }

        public ObservableCollection<PurchaseOrderDetail> SelectedPurchaseOrderDetails
        {
            get => _selectedPurchaseOrderDetails;
            set => _selectedPurchaseOrderDetails = value;
        }

        public ObservableCollection<PurchaseOrder> DisplayedPurchaseOrders
        {
            get => _displayedPurchaseOrders ??= new ObservableCollection<PurchaseOrder>();
            set => _displayedPurchaseOrders = value;
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    CalculateTotalPages();
                }
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    UpdateDisplayedPurchaseOrders();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set => _totalPages = value;
        }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        #endregion

        #region Constructor
        public PurchaseOrderViewModel(PurchaseOrderDatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _allPurchaseOrders = new ObservableCollection<PurchaseOrder>();
            _allPurchaseOrders = new ObservableCollection<PurchaseOrder>();
            _allPurchaseOrders = new ObservableCollection<PurchaseOrder>();
        }
        #endregion

        #region Commands
        [RelayCommand]
        public async Task AddPurchaseOrder(PurchaseOrder newPurchaseOrder)
        {
            if (newPurchaseOrder == null) return;
            await _databaseService.AddPurchaseOrderAsync(newPurchaseOrder);
            await LoadData();
        }

        [RelayCommand]
        public async Task DeletePurchaseOrder()
        {
            if (SelectedPurchaseOrder == null) return;
            try
            {
                await _databaseService.DeletePurchaseOrderAsync(SelectedPurchaseOrder.PurchaseOrderID);
                await LoadData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting order: {ex.Message}");
                throw;
            }
        }

        [RelayCommand]
        public async Task UpdateOrder(PurchaseOrder updatedPurchaseOrder)
        {
            await _databaseService.UpdatePurchaseOrderAsync(updatedPurchaseOrder);
            CalculateTotalPages();
            UpdateDisplayedPurchaseOrders();
        }
        #endregion

        public async Task LoadData()
        {
            await LoadPurchaseOrders();
        }

        private async Task LoadPurchaseOrders()
        {
            var PurchaseOrders = await _databaseService.GetPurchaseOrdersAsync();
            AllPurchaseOrders = new ObservableCollection<PurchaseOrder>(PurchaseOrders);
            CalculateTotalPages();
            UpdateDisplayedPurchaseOrders();
        }

        private async Task LoadPurchaseOrderDetails(int OrderId)
        {
            var OrderDetails = await _databaseService.GetPurchaseOrderDetailAsync(OrderId);
            //SelectedOrderDetails = new ObservableCollection<OrderDetail>(OrderDetails);
        }

        private void CalculateTotalPages()
        {
            TotalPages = (int)Math.Ceiling((double)AllPurchaseOrders.Count / PageSize);
        }

        private void UpdateDisplayedPurchaseOrders()
        {
            var startIndex = (CurrentPage - 1) * PageSize;
            DisplayedPurchaseOrders.Clear();
            for (int i = startIndex; i < startIndex + PageSize && i < AllPurchaseOrders.Count; i++)
            {
                DisplayedPurchaseOrders.Add(AllPurchaseOrders[i]);
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

        public void FilterPuchaseOrdersByDate(DateTime startDate, DateTime endDate)
        {
            var filteredPurchaseOrders = AllPurchaseOrders.Where(po => po.OrderDate >= startDate && po.OrderDate <= endDate).ToList();
            AllPurchaseOrders = new ObservableCollection<PurchaseOrder>(filteredPurchaseOrders);
            CalculateTotalPages();
            UpdateDisplayedPurchaseOrders();
        }

        public void LoadAllPurchaseOrders()
        {
            LoadData().ConfigureAwait(false);
        }

        public async Task LoadSelectedPurchaseOrderDetails()
        {
            if (SelectedPurchaseOrder != null)
            {
                await LoadPurchaseOrderDetails(SelectedPurchaseOrder.PurchaseOrderID);
            }
        }

        public void ResetCurrentPage()
        {
            CurrentPage = 1;
        }
    }
}