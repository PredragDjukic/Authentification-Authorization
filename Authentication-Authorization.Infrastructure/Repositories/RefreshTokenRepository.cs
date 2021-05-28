using Authentication_Authorization.DAL.Entities;
using Authentication_Authorization.DAL.Models.Constants;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Authentication_Authorization.DAL.DatabaseAccess
{
    public class RefreshTokenRepository
    {

        public RefreshToken GetValidRefreshToken(string username, string connectionString)
        {
            RefreshToken invalidRefreshToken = this.GetRefreshToken(
                RefreshTokenStoredProcedures.GetValid,
                username,
                connectionString
            );

            return invalidRefreshToken;
        }

        public RefreshToken GetInvalidRefreshToken(string username, string connectionString)
        {
            RefreshToken invalidRefreshToken = this.GetRefreshToken(
                RefreshTokenStoredProcedures.GetInvalid,
                username,
                connectionString
            );

            return invalidRefreshToken;
        }

        private RefreshToken GetRefreshToken(string type, string username, string connectionString)
        {
            RefreshToken fetchedToken = new();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = type;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = connection;

                    cmd.Parameters.AddWithValue("@Username", username);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            fetchedToken.Token = reader["Token"].ToString();
                            fetchedToken.Username = reader["Username"].ToString();
                            fetchedToken.Expiration = Convert.ToDateTime(reader["Expiration"]);
                        }
                    }
                }
            }

            return fetchedToken;
        }

        public void AddRefreshToken(RefreshToken newRefreshToken, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = RefreshTokenStoredProcedures.Add;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = connection;

                    cmd.Parameters.AddWithValue("@Username", newRefreshToken.Username);
                    cmd.Parameters.AddWithValue("@Token", newRefreshToken.Token);
                    cmd.Parameters.AddWithValue("@Expiration", newRefreshToken.Expiration);


                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateRefreshToken(RefreshToken updatedRefreshToken, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = RefreshTokenStoredProcedures.Update;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = connection;

                    cmd.Parameters.AddWithValue("@Username", updatedRefreshToken.Username);
                    cmd.Parameters.AddWithValue("@Token", updatedRefreshToken.Token);
                    cmd.Parameters.AddWithValue("@Expiration", updatedRefreshToken.Expiration);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
