using PawnShop.Business.Interfaces;
using PawnShop.Oracle.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using PawnShop.Oracle.Extensions;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;

namespace PawnShop.Oracle.Services
{
    public class IdentityService : IService<User>
    {
        private string ConnectionString { get; }
        public IdentityService(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public async Task AddAsync(User model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO users(user_firstname, user_lastname, user_date_of_birth, user_mail, user_password)" +
                          $"VALUES('{model.FirstName}', '{model.LastName}', '{model.DateOfBirth}', '{model.Email}', '{model.Password}')";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(User model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"DELETE FROM users WHERE user_id = {model.Id}";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                var users = new List<User>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT * FROM users";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            var user = ModelConverting.ConvertToObject<User>(dataReader);
                            users.Add(user);
                        }
                    }
                }

                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetByIdAsync(decimal id)
        {
            try
            {
                var user = new User();
                using(OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT * FROM users WHERE user_id = {id}";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            user = ModelConverting.ConvertToObject<User>(dataReader);
                        }
                    }
                }

                return user;
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
                        cmd.CommandText = $"SELECT COUNT(*) FROM users";
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        while (dataReader.Read())
                        {
                            count = int.Parse(dataReader.GetString(0));
                        }
                    }
                }

                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateAsync(User model)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"UPDATE users SET user_firstname = '{model.FirstName}', user_lastname = '{model.LastName}', " +
                            $"user_date_of_birth = '{model.DateOfBirth}', user_mail = '{model.Email}', user_password = '{model.Password}' " +
                            $"WHERE user_id = {model.Id}";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            try
            {
                var user = new User();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT * FROM users WHERE user_mail = '{email}'";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            user = new User
                            {
                                Id = Convert.ToDecimal(dataReader["user_id"]),
                                FirstName = dataReader["user_firstname"].ToString(),
                                LastName = dataReader["user_lastname"].ToString(),
                                Email = dataReader["user_mail"].ToString(),
                                DateOfBirth = dataReader["user_date_of_birth"].ToString(),
                                Password = dataReader["user_password"].ToString()
                            };
                        }
                    }
                }

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> FindByEmailWithRoleAsync(string email)
        {
            try
            {
                var user = new User();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT user_id, user_firstname, user_lastname, user_date_of_birth, user_mail, user_password, role_shortname FROM users " +
                                        "INNER JOIN user_roles ON users.user_id = user_roles.users_user_id " +
                                        "INNER JOIN roles ON user_roles.roles_role_id = roles.role_id " +
                                        $"WHERE user_mail = '{email}'";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            user = new User
                            {
                                Id = Convert.ToDecimal(dataReader["user_id"]),
                                FirstName = dataReader["user_firstname"].ToString(),
                                LastName = dataReader["user_lastname"].ToString(),
                                Email = dataReader["user_mail"].ToString(),
                                DateOfBirth = dataReader["user_date_of_birth"].ToString(),
                                Password = dataReader["user_password"].ToString(),
                                Role = dataReader["role_shortname"].ToString()
                            };
                        }
                    }
                }

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllWithRolesAsync()
        {
            try
            {
                var users = new List<User>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"SELECT user_id, user_firstname, user_lastname, user_date_of_birth, user_mail, user_password, role_shortname FROM users " +
                                        "INNER JOIN user_roles ON users.user_id = user_roles.users_user_id " +
                                        "INNER JOIN roles ON user_roles.roles_role_id = roles.role_id ";
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            var user = new User
                            {
                                Id = Convert.ToDecimal(dataReader["user_id"]),
                                FirstName = dataReader["user_firstname"].ToString(),
                                LastName = dataReader["user_lastname"].ToString(),
                                Email = dataReader["user_mail"].ToString(),
                                DateOfBirth = dataReader["user_date_of_birth"].ToString(),
                                Password = dataReader["user_password"].ToString(),
                                Role = dataReader["role_shortname"].ToString()
                            };

                            users.Add(user);
                        }
                    }
                }

                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            try
            {
                var roles = new List<Role>();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("SELECT role_id, role_fullname, role_shortname FROM ROLES", connection))
                    {
                        await connection.OpenAsync();
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            var role = ModelConverting.ConvertToObject<Role>(dataReader);
                            roles.Add(role);
                        }
                    }
                }

                return roles;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddUserRole(UserRole userRole)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.Connection = connection;
                        cmd.CommandText = $"INSERT INTO user_roles(users_user_id, roles_role_id)" +
                                          $"VALUES({userRole.UserId}, {userRole.RoleId})";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Role> GetRoleByName(string shortRoleName)
        {
            try
            {
                var role = new Role();
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        await connection.OpenAsync();
                        cmd.CommandText = $"SELECT * FROM roles WHERE role_shortname = '{shortRoleName}'";
                        cmd.Connection = connection;
                        OracleDataReader dataReader = await cmd.ExecuteReaderAsync() as OracleDataReader;
                        while (dataReader.Read())
                        {
                            role = ModelConverting.ConvertToObject<Role>(dataReader);
                        }
                    }
                }

                return role;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
