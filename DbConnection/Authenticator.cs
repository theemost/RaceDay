using Microsoft.Data.SqlClient;

namespace RaceDay.DbConnection
{
    public class Authenticator
    {
        Db_connection db = new Db_connection();
        private string connection = @"Data source=(localdb)\RaceDay;Database=RaceDay;";

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
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"select password from Users where username = @username";
                    using (SqlCommand get_user = new SqlCommand(query, connect))
                    {
                        get_user.Parameters.AddWithValue("@username", username);
                        connect.Open();

                        object? result = get_user.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            string storedHash = result.ToString()!;
                            // Verify the password
                            if (BCrypt.Net.BCrypt.Verify(password, result.ToString()!))
                            {
                                message = "User found...\nSuccessfully logged in";
                            }
                            else
                            {
                                message = "Incorrect password...\nPlease check your password and try again";
                            }
                            connect.Close();
                        }
                        else
                        {
                            message = "User not found...\nPlease check your username and password";
                            connect.Close();
                        }
                    }
                }
            }
            catch (Exception error)
            {
                message = error.Message;
            }
            return message;
        }

    }
}
