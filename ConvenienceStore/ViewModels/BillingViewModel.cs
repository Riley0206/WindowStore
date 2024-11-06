using ConvenienceStore.Models;
using ConvenienceStore.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ConvenienceStore.ViewModels
{
    public partial class BillingViewModel : ObservableObject
    {
        #region Private Fields
        private readonly DatabaseService _databaseService;
        private bool _categoriesLoaded;
        private int _pageSize = 10;
        private int _currentPage = 1;
        private int _totalPages;
        private ObservableCollection<Bill> _allBills;
        private ObservableCollection<Bill> _displayedBills;
        private ObservableCollection<DetailedBill> _detailedBills;
        private Bill _selectedBill;
        #endregion

        #region Public Properties
        public ObservableCollection<Bill> AllBills
        {
            get => _allBills;
            set => SetProperty(ref _allBills, value);
        }

        public ObservableCollection<Bill> DisplayedBills
        {
            get => _displayedBills;
            set => SetProperty(ref _displayedBills, value);
        }

        public ObservableCollection<DetailedBill> DetailedBills
        {
            get => _detailedBills;
            set => SetProperty(ref _detailedBills, value);
        }

        public Bill SelectedBill
        {
            get => _selectedBill;
            set => SetProperty(ref _selectedBill, value);
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
                    UpdateDisplayedBills();
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
            _allBills = new ObservableCollection<Bill>();
            _displayedBills = new ObservableCollection<Bill>();
        }
        #endregion

        //#region Commands
        //public IRelayCommand PreviousPageCommand { get; }
        //public IRelayCommand NextPageCommand { get; }
        //#endregion

        #region Methods
        public async Task LoadData()
        {
            //if (!_categoriesLoaded)
            //{
            //    await LoadCategories();
            //    _categoriesLoaded = true;
            //}
            await LoadBills();
        }

        //private async Task LoadCategories()
        //{
        //    var categories = await _databaseService.GetCategoriesAsync();
        //    foreach (var category in categories)
        //    {
        //        Categories.Add(category);
        //    }
        //}

        private async Task LoadBills()
        {
            var bills = await _databaseService.GetBillsAsync();
            foreach (var bill in bills)
            {
                AllBills.Add(bill);
            }
            CalculateTotalPages();
            UpdateDisplayedBills();
        }

        private void CalculateTotalPages()
        {
            TotalPages = (int)Math.Ceiling((double)AllBills.Count / PageSize);
        }

        private void UpdateDisplayedBills()
        {
            var startIndex = (CurrentPage - 1) * PageSize;
            DisplayedBills.Clear();
            for (int i = startIndex; i < startIndex + PageSize && i < AllBills.Count; i++)
            {
                DisplayedBills.Add(AllBills[i]);
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

        public void FilterBillsByDate(DateTime startDate, DateTime endDate)
        {
            var filteredBills = AllBills.Where(b => b.OrderDate >= startDate && b.OrderDate <= endDate).ToList();
            AllBills = new ObservableCollection<Bill>(filteredBills);
            CalculateTotalPages();
            UpdateDisplayedBills();
        }

        public void LoadAllBills()
        {
            LoadData().ConfigureAwait(false);
        }

        public async Task LoadDetailedBills(int purchaseOrderID)
        {
            var detailedBills = await _databaseService.GetDetailedBillAsync(purchaseOrderID);
            DetailedBills = new ObservableCollection<DetailedBill>(detailedBills);
        }
        #endregion
    }
}
