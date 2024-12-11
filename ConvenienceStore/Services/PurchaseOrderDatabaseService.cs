using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using ConvenienceStore.Models;

namespace ConvenienceStore.Services
{
    public class PurchaseOrderDatabaseService
    {
        private readonly string _connectionString;

        public PurchaseOrderDatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<PurchaseOrderDetail> GetPurchaseOrderDetailAsync(int PurchaseOrderID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        @"SELECT * FROM PurchaseOrderDetail WHERE PurchaseOrderID = @PurchaseOrderID", connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new PurchaseOrderDetail
                                {
                                    PurchaseOrderDetailID = reader.GetInt32(reader.GetOrdinal("PurchaseOrderDetailID")),
                                    PurchaseOrderID = reader.GetInt32(reader.GetOrdinal("PurchaseOrderID")),
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
                Debug.WriteLine($"Error in GetPurchaseOrderDetailAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrdersAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        @"SELECT * FROM PurchaseOrder", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            List<PurchaseOrder> purchaseOrders = new List<PurchaseOrder>();
                            while (await reader.ReadAsync())
                            {
                                purchaseOrders.Add(new PurchaseOrder
                                {
                                    PurchaseOrderID = reader.GetInt32(reader.GetOrdinal("PurchaseOrderID")),
                                    SupplierID = reader.GetInt32(reader.GetOrdinal("SupplierID")),
                                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"))
                                });
                            }
                            return purchaseOrders;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetPurchaseOrdersAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<List<PurchaseOrderDetail>> GetPurchaseOrderDetailsAsync(int PurchaseOrderID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        @"SELECT * FROM PurchaseOrderDetail WHERE PurchaseOrderID = @PurchaseOrderID", connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            List<PurchaseOrderDetail> purchaseOrderDetails = new List<PurchaseOrderDetail>();
                            while (await reader.ReadAsync())
                            {
                                purchaseOrderDetails.Add(new PurchaseOrderDetail
                                {
                                    PurchaseOrderDetailID = reader.GetInt32(reader.GetOrdinal("PurchaseOrderDetailID")),
                                    PurchaseOrderID = reader.GetInt32(reader.GetOrdinal("PurchaseOrderID")),
                                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                    UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice"))
                                });
                            }
                            return purchaseOrderDetails;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetPurchaseOrderDetailsAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<int> AddPurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        @"INSERT INTO PurchaseOrder (SupplierID, OrderDate, TotalAmount) 
                        VALUES (@SupplierID, @OrderDate, @TotalAmount); 
                        SELECT SCOPE_IDENTITY();", connection))
                    {
                        command.Parameters.AddWithValue("@SupplierID", purchaseOrder.SupplierID);
                        command.Parameters.AddWithValue("@OrderDate", purchaseOrder.OrderDate);
                        command.Parameters.AddWithValue("@TotalAmount", purchaseOrder.TotalAmount);

                        return Convert.ToInt32(await command.ExecuteScalarAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in AddPurchaseOrderAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task AddPurchaseOrderDetailAsync(PurchaseOrderDetail purchaseOrderDetail)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        @"INSERT INTO PurchaseOrderDetail (PurchaseOrderID, ProductID, Quantity, UnitPrice) 
                        VALUES (@PurchaseOrderID, @ProductID, @Quantity, @UnitPrice);", connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrderDetail.PurchaseOrderID);
                        command.Parameters.AddWithValue("@ProductID", purchaseOrderDetail.ProductID);
                        command.Parameters.AddWithValue("@Quantity", purchaseOrderDetail.Quantity);
                        command.Parameters.AddWithValue("@UnitPrice", purchaseOrderDetail.UnitPrice);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in AddPurchaseOrderDetailAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        @"UPDATE PurchaseOrder 
                        SET SupplierID = @SupplierID, OrderDate = @OrderDate, TotalAmount = @TotalAmount 
                        WHERE PurchaseOrderID = @PurchaseOrderID", connection))
                    {
                        command.Parameters.AddWithValue("@SupplierID", purchaseOrder.SupplierID);
                        command.Parameters.AddWithValue("@OrderDate", purchaseOrder.OrderDate);
                        command.Parameters.AddWithValue("@TotalAmount", purchaseOrder.TotalAmount);
                        command.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrder.PurchaseOrderID);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdatePurchaseOrderAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task UpdatePurchaseOrderDetailAsync(PurchaseOrderDetail purchaseOrderDetail)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        @"UPDATE PurchaseOrderDetail 
                        SET PurchaseOrderID = @PurchaseOrderID, ProductID = @ProductID, Quantity = @Quantity, UnitPrice = @UnitPrice 
                        WHERE PurchaseOrderDetailID = @PurchaseOrderDetailID", connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrderDetail.PurchaseOrderID);
                        command.Parameters.AddWithValue("@ProductID", purchaseOrderDetail.ProductID);
                        command.Parameters.AddWithValue("@Quantity", purchaseOrderDetail.Quantity);
                        command.Parameters.AddWithValue("@UnitPrice", purchaseOrderDetail.UnitPrice);
                        command.Parameters.AddWithValue("@PurchaseOrderDetailID", purchaseOrderDetail.PurchaseOrderDetailID);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdatePurchaseOrderDetailAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task DeletePurchaseOrderAsync(int PurchaseOrderID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        "DELETE FROM PurchaseOrder WHERE PurchaseOrderID = @PurchaseOrderID", connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in DeletePurchaseOrderAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task DeletePurchaseOrderDetailAsync(int PurchaseOrderDetailID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        "DELETE FROM PurchaseOrderDetail WHERE PurchaseOrderDetailID = @PurchaseOrderDetailID", connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseOrderDetailID", PurchaseOrderDetailID);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in DeletePurchaseOrderDetailAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task DeletePurchaseOrderDetailsAsync(int PurchaseOrderID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        "DELETE FROM PurchaseOrderDetail WHERE PurchaseOrderID = @PurchaseOrderID", connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in DeletePurchaseOrderDetailsAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}