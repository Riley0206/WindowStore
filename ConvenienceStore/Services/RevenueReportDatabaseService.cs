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
    public class RevenueReportDatabaseService
    {
        private readonly string _connectionString;
        public RevenueReportDatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            var transactions = new List<Transaction>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("SELECT * FROM [Transaction]", connection))
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
                                ReferenceID = reader.IsDBNull(reader.GetOrdinal("ReferenceID")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("ReferenceID")),
                                ReferenceType = reader.IsDBNull(reader.GetOrdinal("ReferenceType")) ? null : reader.GetString(reader.GetOrdinal("ReferenceType")),
                                PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod"))
                            });
                        }
                    }
                }
            }
            return transactions;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            var orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("SELECT * FROM [Order]", connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var order = new Order
                            {
                                OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                                OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                                PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
                                Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                                Note = reader.GetString(reader.GetOrdinal("Note"))
                            };

                            orders.Add(order);
                        }
                    }
                }

                // Fetch order details for each order
                foreach (var order in orders)
                {
                    order.OrderDetails = await GetOrderDetailsAsync(connection, order.OrderID);
                }
            }

            return orders;
        }
        private async Task<List<OrderDetail>> GetOrderDetailsAsync(SqlConnection connection, int orderId)
        {
            var orderDetails = new List<OrderDetail>();

            using (SqlCommand command = new SqlCommand(
                @"SELECT od.*, p.ProductName, p.Price, p.CostPrice
                  FROM OrderDetail od
                  INNER JOIN Product p ON od.ProductID = p.ProductID
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
                                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                CostPrice = reader.GetDecimal(reader.GetOrdinal("CostPrice"))
                            }
                        });
                    }
                }
            }
            return orderDetails;
        }
    }
}