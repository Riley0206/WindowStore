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
    }
}