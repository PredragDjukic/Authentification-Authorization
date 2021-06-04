using Authentication_Authorization.DAL.Entities;
using Authentication_Authorization.DAL.Interfaces;
using Authentication_Authorization.DAL.Models.Constants;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Authentication_Authorization.DAL.Repositories
{
    public class PlatformCredentialsRepository : IPlatformCredentialsRepository
    {
        private readonly string _connectionString;


        public PlatformCredentialsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public IEnumerable<PlatformCredentials> GetAllPlatformCredentials(int userId)
        {
            List<PlatformCredentials> entities = new();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = PlatformCredentialsStoredProcedures.GetAll;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PlatformCredentials entity = new();

                            entity.Id = Convert.ToInt32(reader["Id"]);
                            entity.Username = reader["Username"].ToString();
                            entity.Name = reader["Name"].ToString();
                            entity.UserId = Convert.ToInt32(reader["UserId"]);
                            entity.ImageName = reader["ImageName"].ToString();

                            entities.Add(entity);
                        }
                    }
                }
            }

            return entities;
        }

        public PlatformCredentials GetPlatformCredentialsById(int userId, int credentialsId)
        {
            PlatformCredentials entity = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = PlatformCredentialsStoredProcedures.GetById;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@Id", credentialsId);
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity = new();

                            entity.Id = Convert.ToInt32(reader["Id"]);
                            entity.Username = reader["Username"].ToString();
                            entity.Name = reader["Name"].ToString();
                            entity.UserId = Convert.ToInt32(reader["UserId"]);
                            entity.ImageName = reader["ImageName"].ToString();
                        }
                    }
                }
            }

            return entity;
        }

        public void AddPlatformCredentials(PlatformCredentials entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = PlatformCredentialsStoredProcedures.Add;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Username", entity.Username);
                    command.Parameters.AddWithValue("@Password", entity.Password);
                    command.Parameters.AddWithValue("@UserId", entity.UserId);
                    command.Parameters.AddWithValue("@CreatedAt", entity.CreatedAt);
                    command.Parameters.AddWithValue("@UpdatedAt", entity.UpdatedAt);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePlatformCredentials(PlatformCredentials entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = PlatformCredentialsStoredProcedures.Update;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@Id", entity.Id);
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Username", entity.Username);
                    command.Parameters.AddWithValue("@Password", entity.Password);
                    command.Parameters.AddWithValue("@UserId", entity.UserId);
                    command.Parameters.AddWithValue("@UpdatedAt", entity.UpdatedAt);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeletePlatformCredentials(int userId, int credentialsId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = PlatformCredentialsStoredProcedures.Delete;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@Id", credentialsId);
                    command.Parameters.AddWithValue("@UserId", userId);

                    command.ExecuteNonQuery();
                }
            }
        }

        public string GetPlatformCredentialsHashedPassword(int userId, int credentialsId)
        {
            string entity = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = PlatformCredentialsStoredProcedures.GetHashedPassword;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@Id", credentialsId);
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity = reader["Password"].ToString();
                        }
                    }
                }
            }

            return entity;
        }

        public void AddImage(int id, int userId, string imageName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = PlatformCredentialsStoredProcedures.InsertImage;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@ImageName", imageName);
                    command.Parameters.AddWithValue("@UserId", userId);

                    command.ExecuteNonQuery();
                }
            }
        }

        public string CheckIfPlatformCredentialsContainImage(int id, int userId)
        {
            string image = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = PlatformCredentialsStoredProcedures.CheckIfImageExist;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            image = reader["ImageName"].ToString();
                        }
                    }
                }
            }

            return image;
        }
    }
}
