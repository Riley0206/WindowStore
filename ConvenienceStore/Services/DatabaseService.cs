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
                Debug.WriteLine("Attempting to connect to database...");
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    Debug.WriteLine("Database connection successful");

                    using (SqlCommand command = new SqlCommand(
                        @"SELECT p.*, c.CategoryName 
                  FROM Product p 
                  JOIN Category c ON p.CategoryID = c.CategoryID", connection))
                    {
                        Debug.WriteLine("Executing SQL query...");
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
                                    ReorderLevel = reader.GetInt32(reader.GetOrdinal("ReorderLevel")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
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
                Debug.WriteLine($"Retrieved {products.Count} products");
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
                    @"INSERT INTO Product (ProductName, CategoryID, Brand, QuantityInStock, ReorderLevel, Price) 
                  VALUES (@ProductName, @CategoryID, @Brand, @QuantityInStock, @ReorderLevel, @Price)", connection))
                {
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                    command.Parameters.AddWithValue("@Brand", product.Brand);
                    command.Parameters.AddWithValue("@QuantityInStock", product.QuantityInStock);
                    command.Parameters.AddWithValue("@ReorderLevel", product.ReorderLevel);
                    command.Parameters.AddWithValue("@Price", product.Price);
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

        public async Task<List<DetailedBill>> GetDetailedBillAsync(int purchaseOrderID)
        {
            var detailedBill = new List<DetailedBill>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    @"SELECT * FROM PurchaseOrderDetail 
              WHERE PurchaseOrderID = @PurchaseOrderID", connection))
                {
                    command.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrderID);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            detailedBill.Add(new DetailedBill
                            {
                                PurchaseOrderID = reader.GetInt32(reader.GetOrdinal("PurchaseOrderID")),
                                PurchaseOrderDetailID = reader.GetInt32(reader.GetOrdinal("PurchaseOrderDetailID")),
                                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                            });
                        }
                    }
                }
            }
            return detailedBill;
        }

        public async Task AddPurchaseOrderAsync(DetailedBill detailedBill)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    @"INSERT INTO PurchaseOrder (PurchaseOrderDate) 
              VALUES (@PurchaseOrderDate); 
              SELECT SCOPE_IDENTITY()", connection))
                {
                    command.Parameters.AddWithValue("@PurchaseOrderDate", DateTime.Now);
                    int purchaseOrderID = Convert.ToInt32(await command.ExecuteScalarAsync());

                    using (SqlCommand detailCommand = new SqlCommand(
                        @"INSERT INTO PurchaseOrderDetail (PurchaseOrderID, ProductID, UnitPrice, Quantity) 
                    VALUES (@PurchaseOrderID, @ProductID, @UnitPrice, @Quantity)", connection))
                    {
                        detailCommand.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrderID);
                        detailCommand.Parameters.AddWithValue("@ProductID", detailedBill.ProductID);
                        detailCommand.Parameters.AddWithValue("@UnitPrice", detailedBill.UnitPrice);
                        detailCommand.Parameters.AddWithValue("@Quantity", detailedBill.Quantity);
                        await detailCommand.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public async Task<List<Bill>> GetBillsAsync()
        {
            var bills = new List<Bill>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("SELECT * FROM PurchaseOrder", connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            bills.Add(new Bill
                            {
                                PurchaseOrderID = reader.GetInt32(reader.GetOrdinal("PurchaseOrderID")),
                                SupplierID = reader.GetInt32(reader.GetOrdinal("SupplierID")),
                                OrderDate = reader.GetDateTime(reader.GetOrdinal("PurchaseOrderDate")),
                                TotalAmount = reader.GetInt32(reader.GetOrdinal("TotalAmount"))
                            });
                        }
                    }
                }
            }
            return bills;
        }

        public async Task DeleteBillAsync(int purchaseOrderID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    "DELETE FROM PurchaseOrder WHERE PurchaseOrderID = @PurchaseOrderID", connection))
                {
                    command.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrderID);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateBillAsync(Bill bill)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(
                    @"UPDATE PurchaseOrder 
              SET SupplierID = @SupplierID, PurchaseOrderDate = @OrderDate, TotalAmount = @TotalAmount 
              WHERE PurchaseOrderID = @PurchaseOrderID", connection))
                {
                    command.Parameters.AddWithValue("@SupplierID", bill.SupplierID);
                    command.Parameters.AddWithValue("@OrderDate", bill.OrderDate);
                    command.Parameters.AddWithValue("@TotalAmount", bill.TotalAmount);
                    command.Parameters.AddWithValue("@PurchaseOrderID", bill.PurchaseOrderID);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}