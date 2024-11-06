using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConvenienceStore.Models;
using ConvenienceStore.Services;

namespace ConvenienceStore.ViewModels
{
    public partial class BillingViewModel : ObservableObject
    {
        #region Private Fields
        private readonly DatabaseService _databaseService;
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
            set => SetProperty(ref _allPurchaseOrders, value);
        }

        public ObservableCollection<PurchaseOrder> DisplayedPurchaseOrders
        {
            get => _displayedPurchaseOrders;
            set => SetProperty(ref _displayedPurchaseOrders, value);
        }

        public ObservableCollection<PurchaseOrderDetail> SelectedPurchaseOrderDetails
        {
            get => _selectedPurchaseOrderDetails;
            set => SetProperty(ref _selectedPurchaseOrderDetails, value);
        }

        public PurchaseOrder SelectedPurchaseOrder
        {
            get => _selectedPurchaseOrder;
            set => SetProperty(ref _selectedPurchaseOrder, value);
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
                    UpdateDisplayedPurchaseOrders();
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
        public BillingViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _allPurchaseOrders = new ObservableCollection<PurchaseOrder>();
            _displayedPurchaseOrders = new ObservableCollection<PurchaseOrder>();
            _selectedPurchaseOrderDetails = new ObservableCollection<PurchaseOrderDetail>();
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task AddPurchaseOrderDetail(PurchaseOrderDetail newPurchaseOrderDetail)
        {
            await _databaseService.AddPurchaseOrderDetailAsync(newPurchaseOrderDetail);
            SelectedPurchaseOrderDetails.Add(newPurchaseOrderDetail);
            CalculateTotalPages();
            UpdateDisplayedPurchaseOrders();
        }

        [RelayCommand]
        private async Task DeletePurchaseOrderDetail(PurchaseOrderDetail purchaseOrderDetail)
        {
            await _databaseService.DeletePurchaseOrderDetailAsync(purchaseOrderDetail.PurchaseOrderDetailID);
            SelectedPurchaseOrderDetails.Remove(purchaseOrderDetail);
            CalculateTotalPages();
            UpdateDisplayedPurchaseOrders();
        }

        [RelayCommand]
        private async Task UpdatePurchaseOrderDetail(PurchaseOrderDetail updatedPurchaseOrderDetail)
        {
            await _databaseService.UpdatePurchaseOrderDetailAsync(updatedPurchaseOrderDetail);
            CalculateTotalPages();
            UpdateDisplayedPurchaseOrders();
        }
        #endregion

        #region Methods
        public async Task LoadData()
        {
            await LoadPurchaseOrders();
        }

        private async Task LoadPurchaseOrders()
        {
            var purchaseOrders = await _databaseService.GetPurchaseOrdersAsync();
            AllPurchaseOrders = new ObservableCollection<PurchaseOrder>(purchaseOrders);
            CalculateTotalPages();
            UpdateDisplayedPurchaseOrders();
        }

        private async Task LoadPurchaseOrderDetails(int purchaseOrderId)
        {
            var purchaseOrderDetails = await _databaseService.GetPurchaseOrderDetailsAsync(purchaseOrderId);
            SelectedPurchaseOrderDetails = new ObservableCollection<PurchaseOrderDetail>(purchaseOrderDetails);
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

        public void FilterPurchaseOrdersByDate(DateTime startDate, DateTime endDate)
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
        #endregion
    }
}