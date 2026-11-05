using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using RaceDay.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RaceDay.DbConnection
{
    public class Authenticator
    {
        private readonly IConfiguration configuration;
        private readonly string connection = @"Data source=(localdb)\RaceDay;Database=RaceDay;";

        public Authenticator(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        //REGISTER
        public string register(string firstName, string lastName, string emailAddress, string username, string password, string role)
        {
            string message = string.Empty;
            try
            {

                //Hash password before storing it in the database
                string passHash = BCrypt.Net.BCrypt.HashPassword(password);
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string query = @"insert into Users (firstName, lastName, emailAddress, username, password, role)
                                     values (@firstName, @lastName, @emailAddress, @username, @password, @role);";
                    using (SqlCommand register_user = new SqlCommand(query, connect))
                    {
                        register_user.Parameters.AddWithValue("@firstName", firstName);
                        register_user.Parameters.AddWithValue("@lastName", lastName);
                        register_user.Parameters.AddWithValue("@emailAddress", emailAddress);
                        register_user.Parameters.AddWithValue("@username", username);
                        register_user.Parameters.AddWithValue("@password", passHash);
                        register_user.Parameters.AddWithValue("@role", role);
                        register_user.ExecuteNonQuery();
                        message = "User registered successfully";
                        connect.Close();
                    }
                }
            }
            catch (Exception error)
            {
                message = error.Message;
            }
            return message;
        }

        public string login(string username, string password)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"
                SELECT userId, password, role
                FROM Users
                WHERE username = @username";

                    using (SqlCommand get_user = new SqlCommand(query, connect))
                    {
                        get_user.Parameters.AddWithValue("@username", username);
                        connect.Open();

                        int user_id;
                        string stored_password;
                        string user_role;

                        using (SqlDataReader reader = get_user.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                return "Invalid username or password.";
                            }

                            user_id = Convert.ToInt32(reader["userId"]);
                            stored_password = reader["password"].ToString()!;
                            user_role = reader["role"].ToString()!;
                        }

                        bool isBcryptHash =
                            stored_password.StartsWith("$2a$") ||
                            stored_password.StartsWith("$2b$") ||
                            stored_password.StartsWith("$2y$");

                        bool passwordValid;

                        if (isBcryptHash)
                        {
                            // Verify an existing BCrypt hash.
                            passwordValid =
                                BCrypt.Net.BCrypt.Verify(password, stored_password);
                        }
                        else
                        {
                            // Temporary compatibility for old plain-text passwords.
                            passwordValid =
                                string.Equals(password, stored_password,
                                    StringComparison.Ordinal);
                        }

                        if (!passwordValid)
                        {
                            return "Invalid username or password.";
                        }

                        if (!isBcryptHash)
                        {
                            // Upgrade the old password to a BCrypt hash.
                            string newHash = BCrypt.Net.BCrypt.HashPassword(password);

                            using (SqlCommand updatePassword = new SqlCommand(
                                @"UPDATE Users
                          SET password = @password
                          WHERE userId = @userId", connect))
                            {
                                updatePassword.Parameters.AddWithValue(
                                    "@password", newHash);
                                updatePassword.Parameters.AddWithValue(
                                    "@userId", user_id);

                                updatePassword.ExecuteNonQuery();
                            }
                        }

                        string jwtKey = configuration["Jwt:Key"]
                            ?? throw new InvalidOperationException(
                                "JWT key is not configured.");

                        var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey));

                        var credentials = new SigningCredentials(
                            key, SecurityAlgorithms.HmacSha256);

                        var claims = new[]
                        {
                    new Claim(ClaimTypes.NameIdentifier, user_id.ToString()),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, user_role)
                };

                        var token = new JwtSecurityToken(
                            issuer: "RaceDay",
                            audience: "RaceDayUsers",
                            claims: claims,
                            expires: DateTime.UtcNow.AddHours(1),
                            signingCredentials: credentials);

                        return new JwtSecurityTokenHandler().WriteToken(token);
                    }
                }
            }
            catch (Exception)
            {
                // Do not expose database or authentication details to the client.
                return "Login failed. Please try again.";
            }
        }

    }
}