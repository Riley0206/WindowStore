using ConvenienceStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ConvenienceStore.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<List<Product>> GetProductsAsync()
        {
            var products = new List<Product>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        @"SELECT p.*, c.CategoryName 
                        FROM Product p 
                        JOIN Category c ON p.CategoryID = c.CategoryID", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                products.Add(new Product
                                {
                                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                    ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                                    CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                    Brand = reader.GetString(reader.GetOrdinal("Brand")),
                                    QuantityInStock = reader.GetInt32(reader.GetOrdinal("QuantityInStock")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    CostPrice = reader.GetDecimal(reader.GetOrdinal("CostPrice")),
                                    Unit = reader.GetString(reader.GetOrdinal("Unit")),
                                    Category = new Category
                                    {
                                        CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                        CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                                    }
                                });
                            }
                        }
                    }
                }
                return products;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetProductsAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var categories = new List<Category>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Category", connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            categories.Add(new Category
                            {
                                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                            });
                        }
                    }
                }
            }
            return categories;
        }
        public async Task AddProductAsync(Product product)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    @"INSERT INTO Product (ProductName, CategoryID, Brand, QuantityInStock, Price, CostPrice, Unit) 
                    VALUES (@ProductName, @CategoryID, @Brand, @QuantityInStock, @Price, @CostPrice, @Unit)", connection))
                {
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                    command.Parameters.AddWithValue("@Brand", product.Brand);
                    command.Parameters.AddWithValue("@QuantityInStock", product.QuantityInStock);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@CostPrice", product.CostPrice);
                    command.Parameters.AddWithValue("@Unit", product.Unit);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task UpdateProductQuantityAsync(int productId, int newQuantity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    "UPDATE Product SET QuantityInStock = @Quantity WHERE ProductID = @ProductID", connection))
                {
                    command.Parameters.AddWithValue("@Quantity", newQuantity);
                    command.Parameters.AddWithValue("@ProductID", productId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteProductAsync(int productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(
                        "DELETE FROM Product WHERE ProductID = @ProductID", connection))
                    {
                        command.Parameters.AddWithValue("@ProductID", productId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in DeleteProductAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<PurchaseOrderDetail>> GetPurchaseOrderDetailsAsync(int purchaseOrderId)
        {
            var purchaseOrderDetails = new List<PurchaseOrderDetail>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    @"SELECT * FROM PurchaseOrderDetail 
                    WHERE PurchaseOrderID = @PurchaseOrderID", connection))
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
                                UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                            });
                        }
                    }
                }
            }
            return purchaseOrderDetails;
        }

        public async Task AddPurchaseOrderDetailAsync(PurchaseOrderDetail purchaseOrderDetail)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand detailCommand = new SqlCommand(
                    @"INSERT INTO PurchaseOrderDetail (PurchaseOrderID, ProductID, UnitPrice, Quantity) 
                    VALUES (@PurchaseOrderID, @ProductID, @UnitPrice, @Quantity)", connection))
                {
                    detailCommand.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrderDetail.PurchaseOrderID);
                    detailCommand.Parameters.AddWithValue("@ProductID", purchaseOrderDetail.ProductID);
                    detailCommand.Parameters.AddWithValue("@UnitPrice", purchaseOrderDetail.UnitPrice);
                    detailCommand.Parameters.AddWithValue("@Quantity", purchaseOrderDetail.Quantity);
                    await detailCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeletePurchaseOrderDetailAsync(int purchaseOrderDetailId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    "DELETE FROM PurchaseOrderDetail WHERE PurchaseOrderDetailID = @PurchaseOrderDetailID", connection))
                {
                    command.Parameters.AddWithValue("@PurchaseOrderDetailID", purchaseOrderDetailId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdatePurchaseOrderDetailAsync(PurchaseOrderDetail purchaseOrderDetail)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    @"UPDATE PurchaseOrderDetail 
                    SET PurchaseOrderID = @PurchaseOrderID, ProductID = @ProductID, 
                    UnitPrice = @UnitPrice, Quantity = @Quantity
                    WHERE PurchaseOrderDetailID = @PurchaseOrderDetailID", connection))
                {
                    command.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrderDetail.PurchaseOrderID);
                    command.Parameters.AddWithValue("@ProductID", purchaseOrderDetail.ProductID);
                    command.Parameters.AddWithValue("@UnitPrice", purchaseOrderDetail.UnitPrice);
                    command.Parameters.AddWithValue("@Quantity", purchaseOrderDetail.Quantity);
                    command.Parameters.AddWithValue("@PurchaseOrderDetailID", purchaseOrderDetail.PurchaseOrderDetailID);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrdersAsync()
        {
            var purchaseOrders = new List<PurchaseOrder>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("SELECT * FROM PurchaseOrder", connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            purchaseOrders.Add(new PurchaseOrder
                            {
                                PurchaseOrderID = reader.GetInt32(reader.GetOrdinal("PurchaseOrderID")),
                                SupplierID = reader.GetInt32(reader.GetOrdinal("SupplierID")),
                                OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                PaymentStatus = reader.GetString(reader.GetOrdinal("PaymentStatus"))
                            });
                        }
                    }
                }
            }
            return purchaseOrders;
        }

        public async Task AddPurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    @"INSERT INTO PurchaseOrder (SupplierID, OrderDate, TotalAmount, Status, PaymentStatus) 
                    VALUES (@SupplierID, @OrderDate, @TotalAmount, @Status, @PaymentStatus); 
                    SELECT SCOPE_IDENTITY()", connection))
                {
                    command.Parameters.AddWithValue("@SupplierID", purchaseOrder.SupplierID);
                    command.Parameters.AddWithValue("@OrderDate", purchaseOrder.OrderDate);
                    command.Parameters.AddWithValue("@TotalAmount", purchaseOrder.TotalAmount);
                    command.Parameters.AddWithValue("@Status", purchaseOrder.Status);
                    command.Parameters.AddWithValue("@PaymentStatus", purchaseOrder.PaymentStatus);
                    int purchaseOrderId = Convert.ToInt32(await command.ExecuteScalarAsync());
                    purchaseOrder.PurchaseOrderID = purchaseOrderId;
                }
            }
        }

        public async Task UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    @"UPDATE PurchaseOrder 
                    SET SupplierID = @SupplierID, OrderDate = @OrderDate, 
                    TotalAmount = @TotalAmount, Status = @Status, 
                    PaymentStatus = @PaymentStatus 
                    WHERE PurchaseOrderID = @PurchaseOrderID", connection))
                {
                    command.Parameters.AddWithValue("@SupplierID", purchaseOrder.SupplierID);
                    command.Parameters.AddWithValue("@OrderDate", purchaseOrder.OrderDate);
                    command.Parameters.AddWithValue("@TotalAmount", purchaseOrder.TotalAmount);
                    command.Parameters.AddWithValue("@Status", purchaseOrder.Status);
                    command.Parameters.AddWithValue("@PaymentStatus", purchaseOrder.PaymentStatus);
                    command.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrder.PurchaseOrderID);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeletePurchaseOrderAsync(int purchaseOrderId)
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
    }
}