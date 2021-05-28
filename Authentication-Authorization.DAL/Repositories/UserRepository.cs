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

        public ICollection<User> GetAllUsers()
        {
            List<User> allUsers = new();

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
                            User fetchedUser = new();
                            fetchedUser.Id = Convert.ToInt32(reader["Id"]);
                            fetchedUser.FullName = reader["FullName"].ToString();
                            fetchedUser.Username = reader["Username"].ToString();
                            fetchedUser.Email = reader["Email"].ToString();
                            fetchedUser.Password = reader["Password"].ToString();
                            fetchedUser.Role = reader["Role"].ToString();
                            allUsers.Add(fetchedUser);
                        }
                    }
                }
            }

            return allUsers;
        }

        public User GetUserById(int id)
        {
            User fetchedUserById = new();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = UserStoredProcedures.GetById;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            fetchedUserById.Id = Convert.ToInt32(reader["Id"]);
                            fetchedUserById.FullName = reader["FullName"].ToString();
                            fetchedUserById.Username = reader["Username"].ToString();
                            fetchedUserById.Email = reader["Email"].ToString();
                            fetchedUserById.Password = reader["Password"].ToString();
                            fetchedUserById.Role = reader["Role"].ToString();
                        }
                    }
                }
            }

            return fetchedUserById;
        }

        public void AddUser(User newUser)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = UserStoredProcedures.Add;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = connection;

                    cmd.Parameters.AddWithValue("@FullName", newUser.FullName);
                    cmd.Parameters.AddWithValue("@Username", newUser.Username);
                    cmd.Parameters.AddWithValue("@Email", newUser.Email);
                    cmd.Parameters.AddWithValue("@Password", newUser.Password);
                    cmd.Parameters.AddWithValue("@Role", newUser.Role);
                    cmd.Parameters.AddWithValue("@CreatedAt", newUser.CreatedAt);
                    cmd.Parameters.AddWithValue("@UpdatedAt", newUser.UpdatedAt);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateUser(User updatedUser)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = UserStoredProcedures.Update;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = connection;

                    cmd.Parameters.AddWithValue("@Id", updatedUser.Id);
                    cmd.Parameters.AddWithValue("@FullName", updatedUser.FullName);
                    cmd.Parameters.AddWithValue("@Username", updatedUser.Username);
                    cmd.Parameters.AddWithValue("@Email", updatedUser.Email);
                    cmd.Parameters.AddWithValue("@Password", updatedUser.Password);
                    cmd.Parameters.AddWithValue("@Role", updatedUser.Role);
                    cmd.Parameters.AddWithValue("@UpdatedAt", updatedUser.UpdatedAt);

                    cmd.ExecuteNonQuery();
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
            User fetchedUserByUsername = new();

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
                            fetchedUserByUsername.Username = reader["Username"].ToString();
                            fetchedUserByUsername.Password = reader["Password"].ToString();
                            fetchedUserByUsername.Role = reader["Role"].ToString();
                            fetchedUserByUsername.Id = Convert.ToInt32(reader["Id"]);
                        }
                    }
                }
            }

            return fetchedUserByUsername;
        }

    }
}
