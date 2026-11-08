using Microsoft.Data.SqlClient;
using RaceDay.Models;
using RaceDay.Data;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.DbConnection
{
    public class UsersConnection
    {

        private readonly RaceDayContext _context;

        public UsersConnection(RaceDayContext context)
        {
            _context = context;
        }

        private string connection = @"Data source=(localdb)\RaceDay;Database=RaceDay;";

        public User? GetUser(int userId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"select * from Users where userId = @userId;";
                    using (SqlCommand get_user = new SqlCommand(query, connect))
                    {
                        get_user.Parameters.AddWithValue("@userId", userId);
                        connect.Open();
                        SqlDataReader reader = get_user.ExecuteReader();
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = (int)reader["userId"],
                                FirstName = reader["firstName"].ToString()!,
                                LastName = reader["lastName"].ToString()!,
                                EmailAddress = reader["emailAddress"].ToString()!,
                                username = reader["username"].ToString()!,
                                Role = reader["role"].ToString()!
                            };
                        }
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the user from the database.", error);
            }
            return null;
        }

        public User? UpdateUser(int userId, string firstName, string lastName, string emailAddress, string username)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"
                UPDATE Users
                SET firstName = @firstName,
                    lastName = @lastName,
                    emailAddress = @emailAddress,
                    username = @username,
                WHERE userId = @userId;";

                    using (SqlCommand user_profile = new SqlCommand(query, connect))
                    {
                        user_profile.Parameters.AddWithValue("@userId", userId);
                        user_profile.Parameters.AddWithValue("@firstName", firstName);
                        user_profile.Parameters.AddWithValue("@lastName", lastName);
                        user_profile.Parameters.AddWithValue("@emailAddress", emailAddress);
                        user_profile.Parameters.AddWithValue("@username", username);
                        connect.Open();

                        int rowsAffected = user_profile.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return null;
                        }
                    }
                }

                // Return the updated profile
                return GetUser(userId);
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to update the user information.", error);
            }
        }
    }
}