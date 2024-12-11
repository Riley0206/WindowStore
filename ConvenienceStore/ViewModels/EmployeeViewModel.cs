using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConvenienceStore.Models;
using ConvenienceStore.Services;
using Microsoft.UI.Xaml.Controls;

namespace ConvenienceStore.ViewModels
{
    public partial class EmployeeViewModel : ObservableObject
    {
        #region Private Fields
        private readonly EmployeeDatabaseService _databaseService;
        private ObservableCollection<Employee> _employees;
        private ObservableCollection<Shift> _shifts;
        private ObservableCollection<Attendance> _attendances;
        private Employee _selectedEmployee;
        private Shift _selectedShift;
        #endregion

        #region Public Properties
        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        public ObservableCollection<Shift> Shifts
        {
            get => _shifts;
            set => SetProperty(ref _shifts, value);
        }

        public ObservableCollection<Attendance> Attendances
        {
            get => _attendances;
            set => SetProperty(ref _attendances, value);
        }

        public string SelectedEmployeeName { get; private set; }
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                if (SetProperty(ref _selectedEmployee, value))
                {
                    // Cập nhật tên nhân viên
                    SelectedEmployeeName = value?.EmployeeName;

                    Task.Run(async () =>
                    {
                        try
                        {
                            if (value != null)
                            {
                                await LoadEmployeeDataAsync();
                            }
                            else
                            {
                                Shifts.Clear();
                                Attendances.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error in SelectedEmployee: {ex.Message}");
                        }
                    });
                }
            }
        }


        public Shift SelectedShift
        {
            get => _selectedShift;
            set => SetProperty(ref _selectedShift, value);
        }
        #endregion

        #region Constructor
        public EmployeeViewModel()
        {
            _databaseService = new EmployeeDatabaseService("Data Source=.\\SQL22;Initial Catalog=ConvenienceStoreDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
            _employees = new ObservableCollection<Employee>();
            _shifts = new ObservableCollection<Shift>();
            _attendances = new ObservableCollection<Attendance>();
            _ = LoadDataAsync();
        }
        #endregion

        #region Commands

        [RelayCommand]
        public async Task MarkAttendanceAsync(Attendance attendance)
        {
            if (SelectedEmployee == null)
            {
                throw new Exception("Hãy chọn một nhân viên trước khi điểm danh!");
            }
            if (attendance == null ||
                attendance.Date == default ||
                string.IsNullOrWhiteSpace(attendance.Status) ||
                attendance.TimeIn == default ||
                attendance.TimeOut == default)
            {
                throw new Exception("Thông tin điểm danh không hợp lệ!");
            }
            try
            {
                attendance.EmployeeID = SelectedEmployee.EmployeeID;
                await _databaseService.MarkAttendanceAsync(attendance);
                await LoadAttendancesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] MarkAttendanceAsync: {ex.Message}");
                throw new Exception($"Không thể điểm danh: {ex.Message}");
            }
        }


        [RelayCommand]
        public async Task AssignShiftAsync(Shift newShift)
        {
            if (SelectedEmployee == null)
            {
                throw new InvalidOperationException("Chưa chọn nhân viên");
            }

            if (newShift == null)
            {
                throw new ArgumentNullException(nameof(newShift), "Thông tin ca làm không được null");
            }

            // Thiết lập giờ làm việc theo ca cố định
            switch (newShift.Note)
            {
                case "Ca 1":
                    newShift.StartTime = TimeSpan.FromHours(7);
                    newShift.EndTime = TimeSpan.FromHours(11);
                    break;
                case "Ca 2":
                    newShift.StartTime = TimeSpan.FromHours(11);
                    newShift.EndTime = TimeSpan.FromHours(15);
                    break;
                case "Ca 3":
                    newShift.StartTime = TimeSpan.FromHours(15);
                    newShift.EndTime = TimeSpan.FromHours(19);
                    break;
                case "Ca 4":
                    newShift.StartTime = TimeSpan.FromHours(19);
                    newShift.EndTime = TimeSpan.FromHours(23);
                    break;
                default:
                    throw new InvalidOperationException("Chọn ca làm không hợp lệ");
            }

            try
            {
                newShift.EmployeeID = SelectedEmployee.EmployeeID;
                await _databaseService.AssignShiftAsync(newShift);
                await LoadShiftsAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] AssignShiftAsync: {ex.Message}");
                throw;
            }
        }

        [RelayCommand]
        public async Task AddEmployeeAsync(Employee newEmployee)
        {
            if (newEmployee == null ||
                string.IsNullOrWhiteSpace(newEmployee.EmployeeName) ||
                string.IsNullOrWhiteSpace(newEmployee.Position) ||
                string.IsNullOrWhiteSpace(newEmployee.PhoneNumber) ||
                string.IsNullOrWhiteSpace(newEmployee.Email) ||
                string.IsNullOrWhiteSpace(newEmployee.IDNumber))
            {
                throw new ArgumentException("Thông tin nhân viên không hợp lệ.");
            }

            try
            {
                await _databaseService.AddEmployeeAsync(newEmployee);
                Employees.Add(newEmployee); // Cập nhật danh sách nhân viên trong ViewModel
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] AddEmployeeAsync: {ex.Message}");
                throw new Exception("Không thể thêm nhân viên.");
            }
        }

        public async Task LoadAllShiftsAsync()
        {
            try
            {
                var shiftData = await _databaseService.GetAllShiftsAsync();
                App.MainDispatcherQueue.TryEnqueue(() =>
                {
                    Shifts.Clear();
                    foreach (var shift in shiftData)
                    {
                        Shifts.Add(shift);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading all shifts: {ex.Message}");
                throw;
            }
        }

        [RelayCommand]
        public async Task CalculateSalaryAsync()
        {
            if (SelectedEmployee == null)
            {
                throw new Exception("Hãy chọn một nhân viên trước khi tính lương!");
            }

            try
            {
                decimal salary = await _databaseService.CalculateSalaryAsync(SelectedEmployee.EmployeeID);
                await new ContentDialog
                {
                    Title = "Kết Quả Tính Lương",
                    Content = $"Lương của nhân viên {SelectedEmployee.EmployeeName}: {salary:C}",
                    CloseButtonText = "OK"
                }.ShowAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] CalculateSalaryAsync: {ex.Message}");
                throw new Exception("Không thể tính lương. Vui lòng thử lại sau.");
            }
        }
        #endregion

        #region Data Loading Methods
        public async Task LoadDataAsync()
        {
            try
            {
                await LoadEmployeesAsync();
                await LoadShiftsAsync();
                await LoadAttendancesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                throw;
            }
        }

        private async Task LoadEmployeesAsync()
        {
            try
            {
                var employeeData = await _databaseService.GetEmployeesAsync();
                Employees = new ObservableCollection<Employee>(employeeData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading employees: {ex.Message}");
                throw new Exception("Không thể tải danh sách nhân viên.");
            }
        }

        public async Task LoadShiftsAsync()
        {
            try
            {
                var shiftData = SelectedEmployee != null
                    ? await _databaseService.GetShiftsByEmployeeAsync(SelectedEmployee.EmployeeID)
                    : await _databaseService.GetAllShiftsAsync();

                App.MainDispatcherQueue.TryEnqueue(() =>
                {
                    Shifts.Clear();
                    foreach (var shift in shiftData)
                    {
                        Shifts.Add(shift);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading shifts: {ex.Message}");
            }
        }

        public async Task LoadAttendancesAsync()
        {
            if (SelectedEmployee != null)
            {
                try
                {
                    var attendanceData = await _databaseService.GetAttendancesByEmployeeAsync(SelectedEmployee.EmployeeID);
                    App.MainDispatcherQueue.TryEnqueue(() =>
                    {
                        Attendances.Clear();
                        foreach (var attendance in attendanceData)
                        {
                            Attendances.Add(attendance);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading attendances: {ex.Message}");
                }
            }
            else
            {
                Attendances.Clear();
            }
        }

        #endregion

        private async Task LoadEmployeeDataAsync()
        {
            try
            {
                if (SelectedEmployee != null)
                {
                    await Task.WhenAll(
                        LoadShiftsAsync(),
                        LoadAttendancesAsync()
                    );
                }
                else
                {
                    Shifts.Clear();
                    Attendances.Clear();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading employee data: {ex.Message}");
            }
        }
    }
}
