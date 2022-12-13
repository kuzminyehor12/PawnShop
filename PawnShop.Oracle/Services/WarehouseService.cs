using Oracle.ManagedDataAccess.Client;
using PawnShop.Business.Interfaces;
using PawnShop.Oracle.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PawnShop.Oracle.Services
{
    public class WarehouseService : IService<Warehouse>
    {
        public string ConnectionString { get; }
        public StringBuilder Html { get; }
        public WarehouseService(string connectionString)
        {
            ConnectionString = connectionString;
            Html = new StringBuilder();
        }
        public async Task AddAsync(Warehouse model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO warehouses(warehouse_capacity, warehouse_size, warehouse_address_id)" +
                          $"VALUES({model.Capacity}, {model.Size}, {model.AddressId})";
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

        public async Task AddAddressAsync(Address model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO addresses(address_country, address_city, address_street, address_number)" +
                          $"VALUES('{model.Country}', '{model.City}', '{model.Street}', '{model.Number}')";
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

        public async Task DeleteAsync(Warehouse model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"DELETE FROM warehouses WHERE warehouse_id = {model.Id}";
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

        public async Task DeleteAddressAsync(Address model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"DELETE FROM addresses WHERE address_id = {model.Id}";
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

        public async Task<IEnumerable<Warehouse>> GetAllAsync()
        {
            try
            {
                var warehouses = new List<Warehouse>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT * FROM warehouses ORDER BY warehouse_id";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            var warehouse = new Warehouse
                            {
                                Id = Convert.ToDecimal(dataReader["warehouse_id"]),
                                Capacity = Convert.ToInt32(dataReader["warehouse_capacity"]),
                                Size = Convert.ToInt32(dataReader["warehouse_size"]),
                                AddressId = Convert.ToDecimal(dataReader["warehouse_address_id"])
                            };

                            warehouses.Add(warehouse);
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return warehouses;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Warehouse>> GetAllWithAddressAsync()
        {
            try
            {
                var warehouses = new List<Warehouse>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT * FROM warehouses INNER JOIN addresses ON warehouses.warehouse_address_id=addresses.address_id ORDER BY warehouse_id";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            var warehouse = new Warehouse
                            {
                                Id = Convert.ToDecimal(dataReader["warehouse_id"]),
                                Capacity = Convert.ToInt32(dataReader["warehouse_capacity"]),
                                Size = Convert.ToInt32(dataReader["warehouse_size"]),
                                AddressId = Convert.ToDecimal(dataReader["warehouse_address_id"]),
                                Country = dataReader["address_country"].ToString(),
                                City = dataReader["address_city"].ToString(),
                                Street = dataReader["address_street"].ToString(),
                                Number = dataReader["address_number"].ToString()
                            };

                            warehouses.Add(warehouse);
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return warehouses;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Warehouse> GetByIdAsync(decimal id)
        {
            try
            {
                var warehouse = new Warehouse();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT * FROM warehouses WHERE warehouse_id={id}";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            warehouse = new Warehouse
                            {
                                Id = Convert.ToDecimal(dataReader["warehouse_id"]),
                                Capacity = Convert.ToInt32(dataReader["warehouse_capacity"]),
                                Size = Convert.ToInt32(dataReader["warehouse_size"]),
                                AddressId = Convert.ToDecimal(dataReader["warehouse_address_id"])
                            };
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return warehouse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Warehouse> GetByAddressId(decimal addressId)
        {
            try
            {
                var warehouse = new Warehouse();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT * FROM warehouses WHERE warehouse_address_id={addressId}";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            warehouse = new Warehouse
                            {
                                Id = Convert.ToDecimal(dataReader["warehouse_id"]),
                                Capacity = Convert.ToInt32(dataReader["warehouse_capacity"]),
                                Size = Convert.ToInt32(dataReader["warehouse_size"]),
                                AddressId = Convert.ToDecimal(dataReader["warehouse_address_id"])
                            };
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return warehouse;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Address> GetAddressByIdAsync(decimal id)
        {
            try
            {
                var address = new Address();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT * FROM addresses WHERE address_id={id}";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            address = new Address
                            {
                                Id = Convert.ToDecimal(dataReader["address_id"]),
                                Country = dataReader["address_country"].ToString(),
                                City = dataReader["address_city"].ToString(),
                                Street = dataReader["address_street"].ToString(),
                                Number = dataReader["address_number"].ToString(),
                            };
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return address;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<decimal> GetAddressIdByModelAsync(Address model)
        {
            try
            {
                var id = 0m;
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT address_id FROM addresses WHERE address_country='{model.Country}' " +
                            $"AND address_city='{model.City}' AND address_street='{model.Street}' AND address_number='{model.Number}'";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            id = Convert.ToDecimal(dataReader["address_id"]);
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return id;
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
                        cmd.CommandText = $"SELECT COUNT(*) FROM warehouses";
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

        public async Task UpdateAsync(Warehouse model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"UPDATE warehouses SET warehouse_capacity = {model.Capacity}, warehouse_size = {model.Size}, warehouse_address_id = {model.AddressId}" +
                            $"WHERE warehouse_id = {model.Id}";
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

        public async Task UpdateAddressAsync(Address model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"UPDATE addresses SET address_country= '{model.Country}', address_city = '{model.City}', address_street = '{model.Street}', " +
                            $"address_number='{model.Number}' WHERE address_id = {model.Id}";
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

        public async Task ExecutePawnPacking(decimal warehouseId, decimal pawningId)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("PAWN_PACKING", connection))
                    {
                        await connection.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("warehouse_id", OracleDbType.Decimal).Value = warehouseId;
                        cmd.Parameters.Add("pawning_id", OracleDbType.Decimal).Value = pawningId;
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
