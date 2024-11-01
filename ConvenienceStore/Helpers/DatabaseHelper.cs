using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ConvenienceStore.Models;

namespace ConvenienceStore.Helpers
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<ProductModel>> GetProductsAsync()
        {
            var products = new List<ProductModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT ProductID, ProductName, CategoryName, Brand, QuantityInStock, Price FROM Products";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var product = new ProductModel
                                {
                                    ProductID = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    CategoryName = reader.GetString(2),
                                    Brand = reader.GetString(3),
                                    QuantityInStock = reader.GetInt32(4),
                                    Price = reader.GetDecimal(5)
                                };
                                products.Add(product);
                            }
                        }
                    }
                }
                Console.WriteLine($"Total products fetched: {products.Count}");
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
            }
            return products;
        }
    }
}
