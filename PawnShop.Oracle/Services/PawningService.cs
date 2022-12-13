using Oracle.ManagedDataAccess.Client;
using PawnShop.Business.Interfaces;
using PawnShop.Data.Models;
using PawnShop.Oracle.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PawnShop.Oracle.Services
{
    public class PawningService : IService<Pawning>
    {
        public StringBuilder Html { get; }
        public string ConnectionString { get; }
        public PawningService(string connectionString)
        {
            ConnectionString = connectionString;
            Html = new StringBuilder();
        }
        public async Task AddAsync(Pawning model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO pawnings(pawning_description, pawning_submission_date, pawning_return_date, pawning_sum, pawning_comission, " +
                            $"clients_client_id, categories_category_id)" +
                          $"VALUES('{model.Description}', '{model.SubmissionDate}', '{model.ReturnDate}', {model.Sum}, {model.Commision}, " +
                          $"{model.ClientId}, {model.CategoryId})";
                        await cmd.ExecuteNonQueryAsync();
                        Html.AppendLine(cmd.CommandText);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(Pawning model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"DELETE FROM pawnings WHERE pawning_id = {model.Id}";
                        await cmd.ExecuteNonQueryAsync();
                        Html.AppendLine(cmd.CommandText);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Pawning>> GetAllAsync()
        {
            try
            {
                var pawnings = new List<Pawning>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT * FROM pawnings";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            var pawning = new Pawning
                            {
                                Id = Convert.ToDecimal(dataReader["pawning_id"]),
                                Description = dataReader["pawning_description"].ToString(),
                                SubmissionDate = dataReader["pawning_submission_date"].ToString(),
                                ReturnDate = dataReader["pawning_return_date"].ToString(),
                                Sum = Convert.ToDecimal(dataReader["pawning_sum"]),
                                Commision = Convert.ToDecimal(dataReader["pawning_comission"]),
                                WarehouseId = Convert.ToDecimal(dataReader["warehouse_warehouse_id"]),
                                ClientId = Convert.ToDecimal(dataReader["client_client_id"]),
                                CategoryId = Convert.ToDecimal(dataReader["category_category_id"]),
                            };

                            pawnings.Add(pawning);
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return pawnings;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Pawning>> GetAllWithNoAddress()
        {
            try
            {
                var pawnings = new List<Pawning>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT pawning_id, pawning_description, pawning_submission_date, pawning_return_date, pawning_sum," +
                            "pawning_comission, category_name FROM pawnings " +
                            "LEFT JOIN warehouses ON pawnings.warehouses_warehouse_id=warehouses.warehouse_id " +
                            "LEFT JOIN categories ON pawnings.categories_category_id=categories.category_id " + 
                            "WHERE warehouses_warehouse_id IS null " +
                            "ORDER BY pawnings.pawning_id ASC";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            bool isCategoryNull = string.IsNullOrEmpty(dataReader["category_name"].ToString());
                            var pawning = new Pawning
                            {
                                Id = Convert.ToDecimal(dataReader["pawning_id"]),
                                Description = dataReader["pawning_description"].ToString(),
                                SubmissionDate = dataReader["pawning_submission_date"].ToString(),
                                ReturnDate = dataReader["pawning_return_date"].ToString(),
                                Sum = Convert.ToDecimal(dataReader["pawning_sum"]),
                                Commision = Convert.ToDecimal(dataReader["pawning_comission"]),
                                CategoryName = isCategoryNull ? "udefined" : dataReader["category_name"].ToString()
                            };
                            pawnings.Add(pawning);
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return pawnings;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Statistic>> GetCategoriesStatistic()
        {
            try
            {
                var statistics = new List<Statistic>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT category_name," +
                                            "MAX(pawnings.pawning_sum) AS max_price," +
                                            "MIN(pawnings.pawning_sum) AS min_price," +
                                            "AVG(pawnings.pawning_sum) AS avg_price," +
                                            "COUNT(pawnings.pawning_id) AS price_count FROM pawnings " +
                                            "RIGHT JOIN categories ON pawnings.categories_category_id = categories.category_id " +
                                            "GROUP BY categories.category_name";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            bool isMaxPriceNull = string.IsNullOrEmpty(dataReader["max_price"].ToString());
                            var statistic = new Statistic
                            {
                                CategoryName = dataReader["category_name"].ToString(),
                                Max = isMaxPriceNull ? 0 : Convert.ToDecimal(dataReader["max_price"]),
                                Min = isMaxPriceNull ? 0 : Convert.ToDecimal(dataReader["min_price"]),
                                Average = isMaxPriceNull ? 0 : Convert.ToDecimal(dataReader["avg_price"]),
                                Count = isMaxPriceNull ? 0 : Convert.ToInt32(dataReader["price_count"]),
                            };
                            statistics.Add(statistic);
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return statistics;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Pawning>> GetAllWithDetailsAsync()
        {
            try
            {
                var pawnings = new List<Pawning>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT pawning_id, pawning_description, pawning_submission_date, pawning_return_date, pawning_sum," +
                            "pawning_comission, address_country, address_city, address_street, address_number, " +
                            "client_firstname, client_lastname, category_name FROM pawnings " +
                            "LEFT JOIN warehouses ON pawnings.warehouses_warehouse_id=warehouses.warehouse_id " +
                            "LEFT JOIN addresses ON warehouses.warehouse_address_id=addresses.address_id " + 
                            "LEFT JOIN clients ON pawnings.clients_client_id=clients.client_id " +
                            "LEFT JOIN categories ON pawnings.categories_category_id=categories.category_id " +
                            "ORDER BY pawnings.pawning_id ASC";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            bool isAddressNull = string.IsNullOrEmpty(dataReader["address_city"].ToString()); 
                            bool isClientNull = string.IsNullOrEmpty(dataReader["client_firstname"].ToString());
                            bool isCategoryNull = string.IsNullOrEmpty(dataReader["category_name"].ToString());

                            var pawning = new Pawning
                            {
                                Id = Convert.ToDecimal(dataReader["pawning_id"]),
                                Description = dataReader["pawning_description"].ToString(),
                                SubmissionDate = dataReader["pawning_submission_date"].ToString(),
                                ReturnDate = dataReader["pawning_return_date"].ToString(),
                                Sum = Convert.ToDecimal(dataReader["pawning_sum"]),
                                Commision = Convert.ToDecimal(dataReader["pawning_comission"]),
                                WarehouseAddress = isAddressNull ? "undefined" : dataReader["address_country"].ToString() + "," + dataReader["address_city"].ToString() + "," +
                                dataReader["address_street"].ToString() + "," + dataReader["address_number"].ToString(),
                                OwnerName = isClientNull ? "undefined" : dataReader["client_firstname"].ToString() + " " + dataReader["client_lastname"].ToString(),
                                CategoryName = isCategoryNull ? "udefined" : dataReader["category_name"].ToString()
                            };
                            pawnings.Add(pawning);
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return pawnings;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Pawning> GetByIdAsync(decimal id)
        {
            try
            {
                var pawning = new Pawning();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT * FROM pawnings";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            bool isAddressNull = string.IsNullOrEmpty(dataReader["warehouses_warehouse_id"].ToString());
                            bool isClientNull = string.IsNullOrEmpty(dataReader["clients_client_id"].ToString());
                            bool isCategoryNull = string.IsNullOrEmpty(dataReader["categories_category_id"].ToString());
                            pawning = new Pawning
                            {
                                Id = Convert.ToDecimal(dataReader["pawning_id"]),
                                Description = dataReader["pawning_description"].ToString(),
                                SubmissionDate = dataReader["pawning_submission_date"].ToString(),
                                ReturnDate = dataReader["pawning_return_date"].ToString(),
                                Sum = Convert.ToDecimal(dataReader["pawning_sum"]),
                                Commision = Convert.ToDecimal(dataReader["pawning_comission"]),
                                WarehouseId = isAddressNull ? 0m : Convert.ToDecimal(dataReader["warehouses_warehouse_id"]),
                                ClientId = isClientNull ? 0m : Convert.ToDecimal(dataReader["clients_client_id"]),
                                CategoryId = isCategoryNull ? 0m : Convert.ToDecimal(dataReader["categories_category_id"]),
                            };
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return pawning;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetCount()
        {
            try
            {
                var count = 0;
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        connection.Open();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT COUNT(*) FROM pawnings";
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        while (dataReader.Read())
                        {
                            count = int.Parse(dataReader.GetString(0));
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateAsync(Pawning model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"UPDATE pawnings SET pawning_description = '{model.Description}', pawning_submission_date = '{model.SubmissionDate}'," +
                            $"pawning_return_date = '{model.ReturnDate}', pawning_sum = {model.Sum}, pawning_commission = {model.Commision}," +
                            $"warehouses_warehouse_id = {model.WarehouseId}, categories_category_id = {model.CategoryId}, clients_client_id = {model.ClientId}" + 
                            $"WHERE pawning_id = {model.Id}";
                        await cmd.ExecuteNonQueryAsync();
                        Html.AppendLine(cmd.CommandText);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
