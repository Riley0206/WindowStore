using ConvenienceStore.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvenienceStore.Services
{
    public class OrderDatabaseService
    {
        private readonly string _connectionString;

        public OrderDatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<OrderDetail> GetOrderDetailAsync(int OrderID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        @"SELECT * FROM OrderDetail WHERE OrderID = @OrderID", connection))
                    {
                        command.Parameters.AddWithValue("@OrderID", OrderID);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new OrderDetail
                                {
                                    OrderDetailID = reader.GetInt32(reader.GetOrdinal("OrderDetailID")),
                                    OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                    UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice"))
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetOrderDetailAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            var orders = new List<Order>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        @"SELECT o.*, c.CustomerName 
                        FROM [Order] o 
                        JOIN Customer c ON o.CustomerID = c.CustomerID", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                orders.Add(new Order
                                {
                                    OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                    CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                                });
                            }
                        }
                    }
                }
                return orders;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetOrdersAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Order> AddOrderAsync(Order order)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        @"INSERT INTO [Order] (CustomerID, OrderDate, TotalAmount) 
                        VALUES (@CustomerID, @OrderDate, @TotalAmount);
                        SELECT SCOPE_IDENTITY();", connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", order.CustomerID);
                        command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);

                        order.OrderID = Convert.ToInt32(await command.ExecuteScalarAsync());
                    }
                }
                return order;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in AddOrderAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task UpdateOrderAsync(Order order)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        @"UPDATE [Order] 
                        SET CustomerID = @CustomerID, OrderDate = @OrderDate, TotalAmount = @TotalAmount 
                        WHERE OrderID = @OrderID", connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", order.CustomerID);
                        command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                        command.Parameters.AddWithValue("@OrderID", order.OrderID);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateOrderAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        "DELETE FROM [Order] WHERE OrderID = @OrderID", connection))
                    {
                        command.Parameters.AddWithValue("@OrderID", orderId);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in DeleteOrderAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}