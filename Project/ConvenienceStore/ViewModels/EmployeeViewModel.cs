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
using System.Collections.Generic;

namespace ConvenienceStore.ViewModels
{
    public partial class EmployeeViewModel : ObservableObject
    {
        #region Private Fields
        private readonly EmployeeDatabaseService _databaseService;
        private ObservableCollection<Employee> _employees;
        private ObservableCollection<Shift> _shifts;
        private Employee _selectedEmployee;
        private DateTimeOffset _selectedDate;
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

        public string SelectedEmployeeName { get; private set; }
        public Shift SelectedShift
        {
            get => _selectedShift;
            set
            {
                if (SetProperty(ref _selectedShift, value))
                {
                    // You can add logic here if you need to do something when a shift is selected
                }
            }
        }
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                if (SetProperty(ref _selectedEmployee, value))
                {
                    // Cập nhật tên nhân viên
                    SelectedEmployeeName = value?.EmployeeName;
                }
            }
        }
        public DateTimeOffset SelectedDate
        {
            get => _selectedDate;
            set
            {
                SetProperty(ref _selectedDate, value);
                _ = LoadShiftsByDateAsync();
            }
        }
        #endregion

        #region Constructor
        public EmployeeViewModel()
        {
            _databaseService = new EmployeeDatabaseService("Data Source=.\\SQL22;Initial Catalog=ConvenienceStoreDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
            _employees = new ObservableCollection<Employee>();
            _shifts = new ObservableCollection<Shift>();
            _selectedDate = DateTime.Now.Date;
            _ = LoadDataAsync();
        }
        #endregion

        #region Commands
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
        [RelayCommand]
        public async Task DeleteEmployeeAsync()
        {
            if (SelectedEmployee == null)
            {
                throw new Exception("Không có nhân viên nào được chọn để xóa.");
            }
            try
            {
                await _databaseService.DeleteEmployeeAsync(SelectedEmployee.EmployeeID);
                Employees.Remove(SelectedEmployee);
                SelectedEmployee = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] DeleteEmployeeAsync: {ex.Message}");
                throw new Exception($"Không thể xóa nhân viên: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task UpdateEmployeeAsync(Employee updatedEmployee)
        {
            if (updatedEmployee == null ||
                 string.IsNullOrWhiteSpace(updatedEmployee.EmployeeName) ||
               string.IsNullOrWhiteSpace(updatedEmployee.Position) ||
                string.IsNullOrWhiteSpace(updatedEmployee.PhoneNumber) ||
                string.IsNullOrWhiteSpace(updatedEmployee.Email) ||
                  string.IsNullOrWhiteSpace(updatedEmployee.IDNumber))
            {
                throw new ArgumentException("Thông tin nhân viên không hợp lệ.");
            }
            try
            {
                await _databaseService.UpdateEmployeeAsync(updatedEmployee);
                // Update the employee in the collection
                var employeeToUpdate = Employees.FirstOrDefault(e => e.EmployeeID == updatedEmployee.EmployeeID);
                if (employeeToUpdate != null)
                {
                    employeeToUpdate.EmployeeName = updatedEmployee.EmployeeName;
                    employeeToUpdate.Position = updatedEmployee.Position;
                    employeeToUpdate.HireDate = updatedEmployee.HireDate;
                    employeeToUpdate.Salary = updatedEmployee.Salary;
                    employeeToUpdate.PhoneNumber = updatedEmployee.PhoneNumber;
                    employeeToUpdate.Address = updatedEmployee.Address;
                    employeeToUpdate.Email = updatedEmployee.Email;
                    employeeToUpdate.Birthday = updatedEmployee.Birthday;
                    employeeToUpdate.IDNumber = updatedEmployee.IDNumber;

                }
                // Update SelectedEmployee if it's the same instance
                if (SelectedEmployee != null && SelectedEmployee.EmployeeID == updatedEmployee.EmployeeID)
                {
                    SelectedEmployee.EmployeeName = updatedEmployee.EmployeeName;
                    SelectedEmployee.Position = updatedEmployee.Position;
                    SelectedEmployee.HireDate = updatedEmployee.HireDate;
                    SelectedEmployee.Salary = updatedEmployee.Salary;
                    SelectedEmployee.PhoneNumber = updatedEmployee.PhoneNumber;
                    SelectedEmployee.Address = updatedEmployee.Address;
                    SelectedEmployee.Email = updatedEmployee.Email;
                    SelectedEmployee.Birthday = updatedEmployee.Birthday;
                    SelectedEmployee.IDNumber = updatedEmployee.IDNumber;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] UpdateEmployeeAsync: {ex.Message}");
                throw new Exception($"Không thể cập nhật nhân viên: {ex.Message}");
            }
        }
        [RelayCommand]
        public async Task AddShiftAsync(Shift newShift)
        {
            if (newShift == null || newShift.EmployeeID == 0)
            {
                throw new ArgumentException("Thông tin ca làm không hợp lệ.");
            }

            try
            {
                await _databaseService.AssignShiftAsync(newShift);
                await LoadShiftsByDateAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] AddShiftAsync: {ex.Message}");
                throw new Exception("Không thể thêm ca làm.");
            }
        }
        [RelayCommand]
        public async Task DeleteShiftAsync()
        {
            if (SelectedShift == null)
            {
                throw new Exception("Không có ca làm nào được chọn để xóa.");
            }

            try
            {
                await _databaseService.DeleteShiftAsync(SelectedShift.ShiftID);
                await LoadShiftsByDateAsync();
                SelectedShift = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] DeleteShiftAsync: {ex.Message}");
                throw new Exception($"Không thể xóa ca làm: {ex.Message}");
            }
        }
        [RelayCommand]
        public async Task UpdateShiftAsync(Shift updatedShift)
        {
            if (updatedShift == null || updatedShift.EmployeeID == 0)
            {
                throw new ArgumentException("Thông tin ca làm không hợp lệ.");
            }

            try
            {
                await _databaseService.UpdateShiftAsync(updatedShift);
                // Update the shift in the collection
                var shiftToUpdate = Shifts.FirstOrDefault(e => e.ShiftID == updatedShift.ShiftID);
                if (shiftToUpdate != null)
                {
                    shiftToUpdate.EmployeeID = updatedShift.EmployeeID;
                    shiftToUpdate.ShiftDate = updatedShift.ShiftDate;
                    shiftToUpdate.StartTime = updatedShift.StartTime;
                    shiftToUpdate.EndTime = updatedShift.EndTime;
                    shiftToUpdate.Status = updatedShift.Status;
                    shiftToUpdate.Note = updatedShift.Note;

                }
                if (SelectedShift != null && SelectedShift.ShiftID == updatedShift.ShiftID)
                {
                    SelectedShift.EmployeeID = updatedShift.EmployeeID;
                    SelectedShift.ShiftDate = updatedShift.ShiftDate;
                    SelectedShift.StartTime = updatedShift.StartTime;
                    SelectedShift.EndTime = updatedShift.EndTime;
                    SelectedShift.Status = updatedShift.Status;
                    SelectedShift.Note = updatedShift.Note;
                }
                await LoadShiftsByDateAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] UpdateShiftAsync: {ex.Message}");
                throw new Exception($"Không thể cập nhật ca làm: {ex.Message}");
            }
        }
        #endregion

        #region Data Loading Methods
        public async Task LoadDataAsync()
        {
            try
            {
                await LoadEmployeesAsync();
                await LoadShiftsByDateAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                throw;
            }
        }
        public async Task LoadShiftsByDateAsync()
        {
            try
            {
                var shiftData = await _databaseService.GetShiftsByDateAsync(SelectedDate.DateTime);
                Debug.WriteLine($"Number of shifts loaded {shiftData.Count}"); // Add this line
                foreach (var shift in shiftData)
                {
                    Debug.WriteLine($"ShiftDate Type from DB: {shift.ShiftDate.GetType()}");
                    Debug.WriteLine($"ShiftDate Data: {shift.ShiftDate}");
                }
                Shifts = new ObservableCollection<Shift>(shiftData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading shifts by date: {ex.Message}");
                throw new Exception("Không thể tải ca làm.");
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
        #endregion
    }
}