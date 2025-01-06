using ConvenienceStore.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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


        public async Task<List<Order>> GetOrdersAsync()
        {
            var orders = new List<Order>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(
                         @"SELECT o.*, e.EmployeeName
                            FROM [Order] o
                            JOIN Employee e ON o.EmployeeID = e.EmployeeID", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                orders.Add(new Order
                                {
                                    OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                                    EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                                    PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
                                    Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                                    Note = reader.GetString(reader.GetOrdinal("Note")),
                                    Employee = new Employee
                                    {
                                        EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                        EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName"))
                                    }
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
        public async Task<List<PurchaseOrder>> GetPurchaseOrdersAsync()
        {
            var purchaseOrders = new List<PurchaseOrder>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(
                            @"SELECT po.*, s.SupplierName,e.EmployeeName
                            FROM PurchaseOrder po
                            JOIN Supplier s ON po.SupplierID = s.SupplierID
                            JOIN Employee e ON po.EmployeeID = e.EmployeeID", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                purchaseOrders.Add(new PurchaseOrder
                                {
                                    PurchaseOrderID = reader.GetInt32(reader.GetOrdinal("PurchaseOrderID")),
                                    SupplierID = reader.GetInt32(reader.GetOrdinal("SupplierID")),
                                    EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                                    Status = reader.GetString(reader.GetOrdinal("Status")),
                                    PaymentStatus = reader.GetString(reader.GetOrdinal("PaymentStatus")),
                                    Note = reader.GetString(reader.GetOrdinal("Note")),
                                    Supplier = new Supplier
                                    {
                                        SupplierID = reader.GetInt32(reader.GetOrdinal("SupplierID")),
                                        SupplierName = reader.GetString(reader.GetOrdinal("SupplierName"))
                                    },
                                    Employee = new Employee
                                    {
                                        EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                        EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName"))
                                    }
                                });
                            }
                        }
                    }
                }
                return purchaseOrders;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetPurchaseOrdersAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<List<OrderDetail>> GetOrderDetailsAsync(int orderId)
        {
            var orderDetails = new List<OrderDetail>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(
                        @"SELECT od.*, p.ProductName
                            FROM OrderDetail od
                            JOIN Product p ON od.ProductID = p.ProductID
                            WHERE od.OrderID = @OrderID", connection))
                    {
                        command.Parameters.AddWithValue("@OrderID", orderId);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                orderDetails.Add(new OrderDetail
                                {
                                    OrderDetailID = reader.GetInt32(reader.GetOrdinal("OrderDetailID")),
                                    OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                    UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                                    Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                                    Product = new Product
                                    {
                                        ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                        ProductName = reader.GetString(reader.GetOrdinal("ProductName"))
                                    }

                                });
                            }
                        }
                    }
                }
                return orderDetails;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetOrderDetailsAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
        public async Task<List<PurchaseOrderDetail>> GetPurchaseOrderDetailsAsync(int purchaseOrderId)
        {
            var purchaseOrderDetails = new List<PurchaseOrderDetail>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(
                        @"SELECT pod.*, p.ProductName
                            FROM PurchaseOrderDetail pod
                            JOIN Product p ON pod.ProductID = p.ProductID
                            WHERE pod.PurchaseOrderID = @PurchaseOrderID", connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrderId);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                purchaseOrderDetails.Add(new PurchaseOrderDetail
                                {
                                    PurchaseOrderDetailID = reader.GetInt32(reader.GetOrdinal("PurchaseOrderDetailID")),
                                    PurchaseOrderID = reader.GetInt32(reader.GetOrdinal("PurchaseOrderID")),
                                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                    UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                                    Product = new Product
                                    {
                                        ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                        ProductName = reader.GetString(reader.GetOrdinal("ProductName"))
                                    }
                                });
                            }
                        }
                    }
                }
                return purchaseOrderDetails;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetPurchaseOrderDetailsAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            var transactions = new List<Transaction>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(
                            "SELECT * FROM [Transaction]", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                transactions.Add(new Transaction
                                {
                                    TransactionID = reader.GetInt32(reader.GetOrdinal("TransactionID")),
                                    TransactionType = reader.GetString(reader.GetOrdinal("TransactionType")),
                                    Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                    TransactionDate = reader.GetDateTime(reader.GetOrdinal("TransactionDate")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    ReferenceID = reader.IsDBNull(reader.GetOrdinal("ReferenceID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ReferenceID")),
                                    ReferenceType = reader.IsDBNull(reader.GetOrdinal("ReferenceType")) ? null : reader.GetString(reader.GetOrdinal("ReferenceType")),
                                    PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod"))
                                });
                            }
                        }
                    }
                }
                return transactions;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetTransactionsAsync: {ex.Message}");
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
                throw;
            }
        }
        public async Task DeletePurchaseOrderAsync(int purchaseOrderId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(
                        "DELETE FROM PurchaseOrder WHERE PurchaseOrderID = @PurchaseOrderID", connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrderId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in DeletePurchaseOrderAsync: {ex.Message}");
                throw;
            }
        }
    }
}