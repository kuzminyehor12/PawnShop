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
    public class CategoryService : IService<Category>
    {
        public StringBuilder Html { get; }
        public string ConnectionString { get; }
        public CategoryService(string connectionString)
        {
            ConnectionString = connectionString;
            Html = new StringBuilder();
        }
        public async Task AddAsync(Category model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO categories(category_name, category_description)" +
                          $"VALUES('{model.Name}', '{model.Description}')";
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

        public async Task DeleteAsync(Category model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"DELETE FROM categories WHERE category_id = {model.Id}";
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

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            try
            {
                var categories = new List<Category>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT * FROM categories ORDER BY category_id ASC";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            var category = ModelConverting.ConvertToObject<Category>(dataReader);
                            categories.Add(category);
                        }

                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return categories;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetAllWithDetailsAsync()
        {
            try
            {
                var categories = new List<Category>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT * FROM categories ORDER BY category_id ASC";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            var category = ModelConverting.ConvertToObject<Category>(dataReader);
                            categories.Add(category);
                        }
                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return categories;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Category> GetByIdAsync(decimal id)
        {
            try
            {
                var category = new Category();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT * FROM categories WHERE category_id = {id}";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            category = ModelConverting.ConvertToObject<Category>(dataReader);
                        }
                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return category;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Category> GetByNameAsync(string categoryName)
        {
            try
            {
                var category = new Category();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT * FROM categories WHERE category_name = '{categoryName}'";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            category = ModelConverting.ConvertToObject<Category>(dataReader);
                        }
                        Html.AppendLine(cmd.CommandText);
                    }
                }

                return category;
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
                        cmd.CommandText = $"SELECT COUNT(*) FROM categories";
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

        public async Task UpdateAsync(Category model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"UPDATE categories SET category_name = '{model.Name}', category_description = '{model.Description}' " +
                            $"WHERE category_id = {model.Id}";
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
