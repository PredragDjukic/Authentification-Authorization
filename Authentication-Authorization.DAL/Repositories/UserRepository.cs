using Authentication_Authorization.DAL.Entities;
using Authentication_Authorization.DAL.Interfaces;
using Authentication_Authorization.DAL.Models.Constants;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Authentication_Authorization.DAL.DatabaseAccess
{
    public class UserRepository : IUserRepository
    {
        private readonly string connectionString;


        public UserRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public IEnumerable<User> GetAllUsers()
        {
            List<User> entities = new();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = UserStoredProcedures.GetAll;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User entity = new();
                            entity.Id = Convert.ToInt32(reader["Id"]);
                            entity.FullName = reader["FullName"].ToString();
                            entity.Username = reader["Username"].ToString();
                            entity.Email = reader["Email"].ToString();
                            entity.Password = reader["Password"].ToString();
                            entity.Role = Convert.ToInt32(reader["Role"]);
                            entities.Add(entity);
                        }
                    }
                }
            }

            return entities;
        }

        public User GetUserById(int id)
        {
            User entity = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = UserStoredProcedures.GetById;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity = new();
                            entity.Id = Convert.ToInt32(reader["Id"]);
                            entity.FullName = reader["FullName"].ToString();
                            entity.Username = reader["Username"].ToString();
                            entity.Email = reader["Email"].ToString();
                            entity.SecretId = reader["SecretId"].ToString();
                            entity.Password = reader["Password"].ToString();
                            entity.Role = Convert.ToInt32(reader["Role"]);
                        }
                    }
                }
            }

            return entity;
        }

        public void AddUser(User entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = UserStoredProcedures.Add;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@FullName", entity.FullName);
                    command.Parameters.AddWithValue("@Username", entity.Username);
                    command.Parameters.AddWithValue("@Email", entity.Email);
                    command.Parameters.AddWithValue("@SecretId", entity.SecretId);
                    command.Parameters.AddWithValue("@Password", entity.Password);
                    command.Parameters.AddWithValue("@Role", entity.Role);
                    command.Parameters.AddWithValue("@CreatedAt", entity.CreatedAt);
                    command.Parameters.AddWithValue("@UpdatedAt", entity.UpdatedAt);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateUser(User entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = UserStoredProcedures.Update;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@Id", entity.Id);
                    command.Parameters.AddWithValue("@FullName", entity.FullName);
                    command.Parameters.AddWithValue("@Username", entity.Username);
                    command.Parameters.AddWithValue("@Email", entity.Email);
                    command.Parameters.AddWithValue("@Password", entity.Password);
                    command.Parameters.AddWithValue("@Role", entity.Role);
                    command.Parameters.AddWithValue("@UpdatedAt", entity.UpdatedAt);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUser(int deletedUserId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = UserStoredProcedures.Delete;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = connection;

                    cmd.Parameters.AddWithValue("@Id", deletedUserId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public User GetUserByUsername(string username)
        {
            User entity = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = UserStoredProcedures.GetByUsername;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = connection;

                    cmd.Parameters.AddWithValue("@Username", username);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity = new();
                            entity.Username = reader["Username"].ToString();
                            entity.Password = reader["Password"].ToString();
                            entity.Role = Convert.ToInt32(reader["Role"]);
                            entity.Id = Convert.ToInt32(reader["Id"]);
                        }
                    }
                }
            }

            return entity;
        }

    }
}
