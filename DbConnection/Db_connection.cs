using Microsoft.Data.SqlClient;

namespace RaceDay.DbConnection
{
    public class Db_connection
    {
        private string connection = @"Data source=(localdb)\RaceDay;Database=RaceDay;";

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
