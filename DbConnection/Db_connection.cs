using Microsoft.Data.SqlClient;

namespace RaceDay.DbConnection
{
    public class Db_connection
    {
        private string connection = @"Data source=(localdb)\RaceDay;Database=RaceDay;";

        //LOGIN
        public string login(string username, string password)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string query = @"select * from Users where username = @username and password = @password;";
                    using (SqlCommand get_user = new SqlCommand(query, connect)) 
                    {
                        get_user.Parameters.AddWithValue("@username", username);
                        get_user.Parameters.AddWithValue("@password", password);

                        SqlDataReader user = get_user.ExecuteReader();

                        if (user.Read())
                        { 
                            {
                                message += "User found...\nSuccessfully logged in";
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

        //REGISTER
        public string register(string firstName, string lastName, string emailAddress, string username, string password, string role)
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string query = @"insert into Users (firstName, lastName, emailAddress, username, password, role) values (@firstName, @lastName, @emailAddress, @username, @password, @role);";
                    using (SqlCommand register_user = new SqlCommand(query, connect))
                    {
                        register_user.Parameters.AddWithValue("@firstName", firstName);
                        register_user.Parameters.AddWithValue("@lastName", lastName);
                        register_user.Parameters.AddWithValue("@emailAddress", emailAddress);
                        register_user.Parameters.AddWithValue("@username", username);
                        register_user.Parameters.AddWithValue("@password", password);
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

        //EVENTS//EVENTS
        public string GetEvents()
        {
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"select * from Events;";
                    SqlCommand get_events = new SqlCommand(query, connect);
                    connect.Open();
                    SqlDataReader event_data = get_events.ExecuteReader();

                    //Check if database empty
                    if (event_data.HasRows)
                    {//Start of if
                        //get data per column
                        while (event_data.Read())
                        {//start of while
                            message += $"Event Name: {event_data["eventName"]}" + "\n";
                            message += $"Description: {event_data["description"]}" + "\n";
                            message += $"Date: {event_data["eventDate"]}" + "\n";
                            message += $"Location: {event_data["location"]}" + "\n";
                            message += $"Event Type: {event_data["eventTypeId"]}" + "\n";
                            message += $"Organiser: {event_data["organiserId"]}" + "\n";
                            message += "---------------------------" + "\n";
                        }//end of while

                        connect.Close();
                    }//End of if

                    else
                    {
                        message = "No events found";
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
    }
}
