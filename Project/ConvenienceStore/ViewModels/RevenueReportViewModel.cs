using CommunityToolkit.Mvvm.ComponentModel;
using ConvenienceStore.Models;
using ConvenienceStore.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;

namespace ConvenienceStore.ViewModels
{
    public partial class RevenueReportViewModel : ObservableObject
    {
        private readonly RevenueReportDatabaseService _databaseService;
        private readonly DispatcherQueue _dispatcherQueue;
        private ObservableCollection<Transaction> _allTransactions;
        private ObservableCollection<Transaction> _filteredTransactions;
        private ObservableCollection<Transaction> _displayedTransactions;
        private ObservableCollection<Order> _allOrders;

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;
        private int _selectedYear;
        private int _selectedMonth;
        private int _pageSize = 10;
        private int _currentPage = 1;
        private int _totalPages;
        private string _selectedTimeRange = "Tháng";
        private decimal _previousTimeRangeRevenue = 0;
        private decimal _currentTimeRangeRevenue = 0;
        private decimal _previousTimeRangeCost = 0;
        private decimal _currentTimeRangeCost = 0;

        private string _formattedStartDate;
        private string _formattedEndDate;
        private bool _isLoading = false;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string FormattedStartDate
        {
            get => _formattedStartDate;
            set => SetProperty(ref _formattedStartDate, value);
        }

        public string FormattedEndDate
        {
            get => _formattedEndDate;
            set => SetProperty(ref _formattedEndDate, value);
        }

        public decimal PreviousTimeRangeRevenue
        {
            get => _previousTimeRangeRevenue;
            set => SetProperty(ref _previousTimeRangeRevenue, value);
        }

        public decimal CurrentTimeRangeRevenue
        {
            get => _currentTimeRangeRevenue;
            set => SetProperty(ref _currentTimeRangeRevenue, value);
        }

        public decimal PreviousTimeRangeCost
        {
            get => _previousTimeRangeCost;
            set => SetProperty(ref _previousTimeRangeCost, value);
        }

        public decimal CurrentTimeRangeCost
        {
            get => _currentTimeRangeCost;
            set => SetProperty(ref _currentTimeRangeCost, value);
        }

        public ObservableCollection<Transaction> AllTransactions
        {
            get => _allTransactions;
            set => SetProperty(ref _allTransactions, value);
        }


        public ObservableCollection<Transaction> FilteredTransactions
        {
            get => _filteredTransactions;
            set
            {
                if (SetProperty(ref _filteredTransactions, value))
                {
                    CalculateTotalPages();
                    UpdateDisplayedTransactions();
                    OnPropertyChanged(nameof(HasPreviousPage));
                    OnPropertyChanged(nameof(HasNextPage));
                }
            }
        }


        public ObservableCollection<Transaction> DisplayedTransactions
        {
            get => _displayedTransactions;
            set => SetProperty(ref _displayedTransactions, value);
        }

        public ObservableCollection<Order> AllOrders
        {
            get => _allOrders;
            set => SetProperty(ref _allOrders, value);
        }


        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    SelectedMonth = _startDate.Month;
                    SelectedYear = _startDate.Year;
                    UpdateFormattedDates();
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    UpdateFormattedDates();
                }
            }
        }

        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (SetProperty(ref _selectedYear, value))
                {
                    UpdateFormattedDates();
                }
            }
        }

        public int SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (SetProperty(ref _selectedMonth, value))
                {
                    UpdateFormattedDates();
                }
            }
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
                    UpdateDisplayedTransactions();
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
                    UpdateDisplayedTransactions();
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

        public string SelectedTimeRange
        {
            get => _selectedTimeRange;
            set
            {
                if (SetProperty(ref _selectedTimeRange, value))
                {
                    UpdateFormattedDates();
                }
            }
        }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public RevenueReportViewModel(RevenueReportDatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _allTransactions = new ObservableCollection<Transaction>();
            _filteredTransactions = new ObservableCollection<Transaction>();
            _displayedTransactions = new ObservableCollection<Transaction>();
            _allOrders = new ObservableCollection<Order>();
            _selectedYear = DateTime.Now.Year;
            _selectedMonth = DateTime.Now.Month;
            UpdateFormattedDates();
        }

        public async Task LoadData()
        {
            if (!_dispatcherQueue.HasThreadAccess)
            {
                _dispatcherQueue.TryEnqueue(() => _ = LoadData());
                return;
            }

            try
            {
                IsLoading = true;
                var transactionsData = await _databaseService.GetTransactionsAsync();
                var ordersData = await _databaseService.GetOrdersAsync();
                _dispatcherQueue.TryEnqueue(() =>
                {
                    AllTransactions = new ObservableCollection<Transaction>(transactionsData);
                    AllOrders = new ObservableCollection<Order>(ordersData);
                    UpdateFilteredTransactionsAsync();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task UpdateFilteredTransactionsAsync()
        {
            if (!_dispatcherQueue.HasThreadAccess)
            {
                _dispatcherQueue.TryEnqueue(() => _ = UpdateFilteredTransactionsAsync());
                return;
            }

            try
            {
                IsLoading = true;

                if (AllTransactions == null)
                {
                    FilteredTransactions = new ObservableCollection<Transaction>();
                    CalculateTotalPages();
                    UpdateDisplayedTransactions();
                    CalculateRevenueComparison();
                    OnPropertyChanged(nameof(HasPreviousPage));
                    OnPropertyChanged(nameof(HasNextPage));
                    return;
                }

                await Task.Run(() =>
                {
                    var filteredTransactions = AllTransactions.Where(GetFilterPredicate()).ToList();
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        FilteredTransactions = new ObservableCollection<Transaction>(filteredTransactions);
                        CalculateRevenueComparison();
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error filtering data: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                IsLoading = false;
            }
        }


        private void UpdateDisplayedTransactions()
        {
            if (!_dispatcherQueue.HasThreadAccess)
            {
                _dispatcherQueue.TryEnqueue(UpdateDisplayedTransactions);
                return;
            }

            if (FilteredTransactions == null || FilteredTransactions.Count == 0)
            {
                DisplayedTransactions = new ObservableCollection<Transaction>();
                return;
            }

            var skip = (CurrentPage - 1) * PageSize;
            var items = FilteredTransactions.Skip(skip).Take(PageSize).ToList();
            DisplayedTransactions = new ObservableCollection<Transaction>(items);
        }

        private Func<Transaction, bool> GetFilterPredicate()
        {
            return SelectedTimeRange switch
            {
                "Tháng" => transaction =>
                        transaction.TransactionDate.Year == SelectedYear &&
                        transaction.TransactionDate.Month == SelectedMonth,

                "Năm" => transaction =>
                    transaction.TransactionDate.Year == SelectedYear,


                _ => transaction => true
            };
        }

        private void CalculateRevenueComparison()
        {
            try
            {
                if (FilteredTransactions?.Count > 0)
                {
                    CurrentTimeRangeRevenue = FilteredTransactions
                       .Where(t => t.TransactionType == "Thu nhập")
                       .Sum(t => t.Amount);

                    CurrentTimeRangeCost = FilteredTransactions
                        .Where(t => t.TransactionType == "Chi tiêu")
                        .Sum(t => t.Amount);

                    PreviousTimeRangeRevenue = SelectedTimeRange switch
                    {
                        "Tháng" => CalculatePreviousMonthRevenue(),
                        "Năm" => CalculatePreviousYearRevenue(),
                        _ => 0
                    };
                    PreviousTimeRangeCost = SelectedTimeRange switch
                    {
                        "Tháng" => CalculatePreviousMonthCost(),
                        "Năm" => CalculatePreviousYearCost(),
                        _ => 0
                    };
                }
                else
                {
                    CurrentTimeRangeRevenue = 0;
                    PreviousTimeRangeRevenue = 0;
                    CurrentTimeRangeCost = 0;
                    PreviousTimeRangeCost = 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error calculating revenue comparison: {ex.Message}");
                CurrentTimeRangeRevenue = 0;
                PreviousTimeRangeRevenue = 0;
                CurrentTimeRangeCost = 0;
                PreviousTimeRangeCost = 0;
            }
        }


        private decimal CalculatePreviousMonthRevenue()
        {
            var previousMonthStart = new DateTime(SelectedYear, SelectedMonth, 1).AddMonths(-1);
            var previousMonthEnd = previousMonthStart.AddMonths(1).AddDays(-1);
            return AllTransactions
                 .Where(t => t.TransactionType == "Thu nhập" &&
                             t.TransactionDate >= previousMonthStart &&
                            t.TransactionDate <= previousMonthEnd)
                .Sum(t => t.Amount);
        }
        private decimal CalculatePreviousMonthCost()
        {
            var previousMonthStart = new DateTime(SelectedYear, SelectedMonth, 1).AddMonths(-1);
            var previousMonthEnd = previousMonthStart.AddMonths(1).AddDays(-1);
            return AllTransactions
                .Where(t => t.TransactionType == "Chi tiêu" &&
                            t.TransactionDate >= previousMonthStart &&
                           t.TransactionDate <= previousMonthEnd)
               .Sum(t => t.Amount);
        }

        private decimal CalculatePreviousYearRevenue()
        {
            var previousYearStart = new DateTime(SelectedYear, 1, 1).AddYears(-1);
            var previousYearEnd = new DateTime(previousYearStart.Year, 12, 31);
            return AllTransactions
                  .Where(t => t.TransactionType == "Thu nhập" &&
                              t.TransactionDate.Year == previousYearStart.Year &&
                             t.TransactionDate <= previousYearEnd)
                .Sum(t => t.Amount);
        }
        private decimal CalculatePreviousYearCost()
        {
            var previousYearStart = new DateTime(SelectedYear, 1, 1).AddYears(-1);
            var previousYearEnd = new DateTime(previousYearStart.Year, 12, 31);
            return AllTransactions
               .Where(t => t.TransactionType == "Chi tiêu" &&
                             t.TransactionDate.Year == previousYearStart.Year &&
                            t.TransactionDate <= previousYearEnd)
               .Sum(t => t.Amount);
        }

        private void UpdateFormattedDates()
        {
            FormattedStartDate = SelectedTimeRange switch
            {

                "Tháng" => SelectedYear != 0 && SelectedMonth != 0 ? new DateTime(SelectedYear, SelectedMonth, 1).ToString("MM/yyyy") : "N/A",
                "Năm" => SelectedYear != 0 ? new DateTime(SelectedYear, 1, 1).ToString("yyyy") : "N/A",
                _ => StartDate.ToString("dd/MM/yyyy")
            };

            FormattedEndDate = SelectedTimeRange switch
            {
                "Tháng" => SelectedYear != 0 && SelectedMonth != 0 ? new DateTime(SelectedYear, SelectedMonth, DateTime.DaysInMonth(SelectedYear, SelectedMonth)).ToString("MM/yyyy") : "N/A",
                "Năm" => SelectedYear != 0 ? new DateTime(SelectedYear, 12, 31).ToString("yyyy") : "N/A",
                _ => EndDate.ToString("dd/MM/yyyy")
            };
        }
        private void CalculateTotalPages()
        {
            var itemCount = FilteredTransactions?.Count ?? 0;
            TotalPages = (itemCount + PageSize - 1) / PageSize;
            if (CurrentPage > TotalPages)
            {
                CurrentPage = Math.Max(1, TotalPages);
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
    }
}