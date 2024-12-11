using System;

namespace ConvenienceStore.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public string IDNumber { get; set; }
    }

    public class Shift
    {
        public int ShiftID { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeID { get; set; }
        public DateTime ShiftDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }

    public class Attendance
    {
        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public string Note { get; set; }
    }
}
