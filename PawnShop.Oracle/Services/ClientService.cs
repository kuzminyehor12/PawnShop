using Oracle.ManagedDataAccess.Client;
using PawnShop.Business.Interfaces;
using PawnShop.Data.Models;
using PawnShop.Oracle.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PawnShop.Oracle.Services
{
    public class ClientService : IService<Client>
    {
        public StringBuilder Html { get; }
        public string ConnectionString { get; }
        public ClientService(string connectionString)
        {
            ConnectionString = connectionString;
            Html = new StringBuilder();
        }
        public async Task AddAsync(Client model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO clients(client_firstname, client_lastname)" +
                          $"VALUES('{model.FirstName}', '{model.LastName}')";
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

        public async Task AddPassportAsync(Passport model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO passports(passport_number, passport_series, passport_date_of_issue, clients_client_id)" +
                          $"VALUES('{model.Number}', '{model.Series}', '{model.DateOfIssue}', {model.ClientId})";
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

        public async Task DeleteAsync(Client model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"DELETE FROM clients WHERE client_id = {model.Id}";
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

        public async Task DeletePassportAsync(Passport model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"DELETE FROM passports WHERE passport_id = {model.Id}";
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

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            try
            {
                var clients = new List<Client>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT * FROM clients";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            var client = new Client
                            {
                                Id = Convert.ToDecimal(dataReader["client_id"]),
                                FirstName = dataReader["client_firstname"].ToString(),
                                LastName = dataReader["client_lastname"].ToString()
                            };
                            clients.Add(client);
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return clients;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Client>> GetAllWithPassportAsync()
        {
            try
            {
                var clients = new List<Client>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT clients.client_id, client_firstname, client_lastname, passport_number, passport_series, passport_date_of_issue FROM clients " +
                            "INNER JOIN passports ON clients.client_id=passports.clients_client_id ORDER BY client_id ASC";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            var client = new Client
                            {
                                Id = Convert.ToDecimal(dataReader["client_id"]),
                                FirstName = dataReader["client_firstname"].ToString(),
                                LastName = dataReader["client_lastname"].ToString(),
                                Number = dataReader["passport_number"].ToString(),
                                Series = dataReader["passport_series"].ToString(),
                                DateOfIssue = dataReader["passport_date_of_issue"].ToString(),
                            };

                            clients.Add(client);
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return clients;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Client> GetByIdAsync(decimal id)
        {
            try
            {
                var client = new Client();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT * FROM clients WHERE client_id = {id}";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            client = new Client
                            {
                                Id = id,
                                FirstName = dataReader["client_firstname"].ToString(),
                                LastName = dataReader["client_lastname"].ToString()
                            };
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return client;
            }
            catch (Exception)
            {
                throw;
            }
        }

  
        public async Task<Passport> GetByPassportNumber(string passportNumber)
        {
            try
            {
                var passport = new Passport();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT * FROM passports WHERE passport_number = {passportNumber}";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            passport = new Passport
                            {
                                Id = Convert.ToDecimal(dataReader["passport_id"]),
                                ClientId = Convert.ToDecimal(dataReader["clients_client_id"]),
                                Number = dataReader["passport_number"].ToString(),
                                Series = dataReader["passport_series"].ToString(),
                                DateOfIssue = dataReader["passport_date_of_issue"].ToString()
                            };
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return passport;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Client> GetByFullName(string fullname)
        {
            try
            {
                var client = new Client();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        var names = fullname.Split(' ');
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT * FROM clients WHERE client_firstname = '{names[0]}' AND client_lastname = '{names[1]}'";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            client = new Client
                            { 
                                Id = Convert.ToDecimal(dataReader["client_id"]),
                                FirstName = dataReader["client_firstname"].ToString(),
                                LastName = dataReader["client_lastname"].ToString()
                            };
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return client;
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
                        cmd.CommandText = $"SELECT COUNT(*) FROM clients";
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

        public async Task UpdateAsync(Client model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"UPDATE clients SET client_firstname = '{model.FirstName}', client_lastname = '{model.LastName}' " +
                            $"WHERE client_id = {model.Id}";
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

        public async Task UpdatePassportAsync(Passport model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"UPDATE passports SET passport_number = '{model.Number}', passport_series = '{model.Series}', passport_date_of_issue = '{model.DateOfIssue}'" +
                            $"WHERE passport_id = {model.Id}";
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
