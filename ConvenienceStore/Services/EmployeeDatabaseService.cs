using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using ConvenienceStore.Models;

namespace ConvenienceStore.Services
{
    public class EmployeeDatabaseService
    {
        private readonly string _connectionString;

        public EmployeeDatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            var employees = new List<Employee>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM Employee";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employees.Add(new Employee
                            {
                                EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                                Position = reader.GetString(reader.GetOrdinal("Position")),
                                HireDate = reader.GetDateTime(reader.GetOrdinal("HireDate")),
                                Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Birthday = reader.GetDateTime(reader.GetOrdinal("Birthday")),
                                IDNumber = reader.GetString(reader.GetOrdinal("IDNumber"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] GetEmployeesAsync: {ex.Message}");
                throw new Exception("Không thể tải danh sách nhân viên. Vui lòng kiểm tra kết nối cơ sở dữ liệu.");
            }

            return employees;
        }

        public async Task<List<Shift>> GetShiftsByEmployeeAsync(int employeeId)
        {
            var shifts = new List<Shift>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM Shift WHERE EmployeeID = @EmployeeID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", employeeId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                shifts.Add(new Shift
                                {
                                    ShiftID = reader.GetInt32(reader.GetOrdinal("ShiftID")),
                                    EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                    ShiftDate = reader.GetDateTime(reader.GetOrdinal("ShiftDate")),
                                    StartTime = reader.GetTimeSpan(reader.GetOrdinal("StartTime")),
                                    EndTime = reader.GetTimeSpan(reader.GetOrdinal("EndTime")),
                                    Status = reader.GetString(reader.GetOrdinal("Status")),
                                    Note = reader.GetString(reader.GetOrdinal("Note"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetShiftsByEmployeeAsync: {ex.Message}");
                throw;
            }

            return shifts;
        }

        public async Task<List<Shift>> GetAllShiftsAsync()
        {
            var shifts = new List<Shift>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"SELECT s.*, e.EmployeeName 
                          FROM Shift s 
                          JOIN Employee e ON s.EmployeeID = e.EmployeeID";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            shifts.Add(new Shift
                            {
                                ShiftID = reader.GetInt32(reader.GetOrdinal("ShiftID")),
                                EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                ShiftDate = reader.GetDateTime(reader.GetOrdinal("ShiftDate")),
                                StartTime = reader.GetTimeSpan(reader.GetOrdinal("StartTime")),
                                EndTime = reader.GetTimeSpan(reader.GetOrdinal("EndTime")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                Note = reader.GetString(reader.GetOrdinal("Note")),
                                EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")) // Thêm trường này
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetAllShiftsAsync: {ex.Message}");
                throw;
            }

            return shifts;
        }

        public async Task MarkAttendanceAsync(Attendance attendance)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "INSERT INTO Attendance (EmployeeID, Date, Status, TimeIn, TimeOut, Note) " +
                                "VALUES (@EmployeeID, @Date, @Status, @TimeIn, @TimeOut, @Note)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", attendance.EmployeeID);
                        command.Parameters.AddWithValue("@Date", attendance.Date);
                        command.Parameters.AddWithValue("@Status", attendance.Status);
                        command.Parameters.AddWithValue("@TimeIn", attendance.TimeIn ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TimeOut", attendance.TimeOut ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Note", string.IsNullOrWhiteSpace(attendance.Note) ? (object)DBNull.Value : attendance.Note);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in MarkAttendanceAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<decimal> CalculateSalaryAsync(int employeeId)
        {
            decimal totalSalary = 0;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "EXEC CalculateSalary @EmployeeID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", employeeId);
                        totalSalary = (decimal)await command.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CalculateSalaryAsync: {ex.Message}");
                throw;
            }

            return totalSalary;
        }

        public async Task<List<Attendance>> GetAttendancesByEmployeeAsync(int employeeId)
        {
            var attendances = new List<Attendance>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM Attendance WHERE EmployeeID = @EmployeeID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", employeeId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                attendances.Add(new Attendance
                                {
                                    AttendanceID = reader.GetInt32(reader.GetOrdinal("AttendanceID")),
                                    EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                    Status = reader.GetString(reader.GetOrdinal("Status")),
                                    TimeIn = reader.IsDBNull(reader.GetOrdinal("TimeIn")) ? null : reader.GetTimeSpan(reader.GetOrdinal("TimeIn")),
                                    TimeOut = reader.IsDBNull(reader.GetOrdinal("TimeOut")) ? null : reader.GetTimeSpan(reader.GetOrdinal("TimeOut")),
                                    Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note")) // Thay đổi này
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetAttendancesByEmployeeAsync: {ex.Message}");
                throw;
            }

            return attendances;
        }
        public async Task AssignShiftAsync(Shift shift)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "INSERT INTO Shift (EmployeeID, ShiftDate, StartTime, EndTime, Status, Note) VALUES (@EmployeeID, @ShiftDate, @StartTime, @EndTime, @Status, @Note)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", shift.EmployeeID);
                        command.Parameters.AddWithValue("@ShiftDate", shift.ShiftDate);
                        command.Parameters.AddWithValue("@StartTime", shift.StartTime);
                        command.Parameters.AddWithValue("@EndTime", shift.EndTime);
                        command.Parameters.AddWithValue("@Status", shift.Status);
                        command.Parameters.AddWithValue("@Note", shift.Note);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in AssignShiftAsync: {ex.Message}");
                throw;
            }
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
            INSERT INTO Employee (EmployeeName, Position, HireDate, Salary, PhoneNumber, Address, Email, Birthday, IDNumber) 
            VALUES (@Name, @Position, @HireDate, @Salary, @PhoneNumber, @Address, @Email, @Birthday, @IDNumber)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", employee.EmployeeName);
                command.Parameters.AddWithValue("@Position", employee.Position);
                command.Parameters.AddWithValue("@HireDate", employee.HireDate);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber);
                command.Parameters.AddWithValue("@Address", employee.Address);
                command.Parameters.AddWithValue("@Email", employee.Email);
                command.Parameters.AddWithValue("@Birthday", employee.Birthday);
                command.Parameters.AddWithValue("@IDNumber", employee.IDNumber);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

    }
}