using Microsoft.Data.SqlClient;
using RaceDay.Models;

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
                    string query = @"insert into Users (firstName, lastName, emailAddress, username, password, role)
                                     values (@firstName, @lastName, @emailAddress, @username, @password, @role);";
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

        //EVENTS****EVENTS
        public List<Event> GetEvents()
        {
            List<Event> events = new List<Event>();
            string message = string.Empty;
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"select	eventId as 'Event ID',
                                            eventName as 'Event Name',
		                                    description as 'Description',
		                                    eventDate as 'Date',
		                                    location as 'Location',
		                                    EventTypes.eventTypeName as 'Event Type',
		                                    Users.firstName + ' ' + Users.lastName as Organiser
                                    from Events 
                                    inner join Users on Users.userId = Events.organiserId
                                    inner join EventTypes on EventTypes.eventTypeId = Events.eventTypeId;";
                    SqlCommand event_data = new SqlCommand(query, connect);
                    connect.Open();
                    SqlDataReader reader = event_data.ExecuteReader();

                    //Check if database empty
                    if (reader.HasRows)
                    {
                        //get data per column
                        while (reader.Read())
                        {
                            events.Add(new Event
                            {
                                EventId = (int)reader["Event Id"],
                                EventName = reader["Event Name"].ToString()!,
                                Description = reader["Description"].ToString()!,
                                EventDate = reader["Date"].ToString()!,
                                Location = reader["Location"].ToString()!,
                                EventType = reader["Event Type"].ToString()!,
                                Organiser = reader["Organiser"].ToString()!
                            });
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
            catch (SqlException error)
            {
                // Log the error appropriately in a real application.
                throw new InvalidOperationException(
                    "Unable to retrieve the event from the database.", error);
            }
            return events;
        }

        public Event? GetEvent(int eventId)
        {//GetEvent
            string message = string.Empty;

            try
            {//Try
                using (SqlConnection connect = new SqlConnection(connection))
                {//using
                    connect.Open();
                        
                    string query = @"select	eventName as 'Event Name',
		                                    description as 'Description',
		                                    eventDate as 'Date',
		                                    location as 'Location',
		                                    EventTypes.eventTypeName as 'Event Type',
		                                    Users.firstName + ' ' + Users.lastName as Organiser
                                    from Events 
                                    inner join Users on Users.userId = Events.organiserId
                                    inner join EventTypes on EventTypes.eventTypeId = Events.eventTypeId
                                    where Events.eventId = @eventId;";
                    using (SqlCommand get_event = new SqlCommand(query, connect))
                    {
                        get_event.Parameters.AddWithValue("@eventId", eventId);
                        SqlDataReader reader = get_event.ExecuteReader();
                        
                        if (reader.Read())
                        {
                            return new Event
                            {
                                EventId = @eventId,
                                EventName = reader["Event Name"].ToString()!,
                                Description = reader["Description"].ToString()!,
                                EventDate = reader["Date"].ToString()!,
                                Location = reader["Location"].ToString()!,
                                EventType = reader["Event Type"].ToString()!,
                                Organiser = reader["Organiser"].ToString()!
                            };
                        }
                        else
                        {
                            message = "Event not found";
                        }
                    }
                }//using
            }//Try
            catch (SqlException error)
            {
                // Log the error appropriately in a real application.
                throw new InvalidOperationException(
                    "Unable to retrieve the event from the database.", error);
            }
            return null;
        }

        public Event? AddEvent(string eventName, string description, DateTime eventDate, string location, int eventTypeId, int organiserId)
        {
            string message = "error adding event....";
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string query = @"insert into Events(eventName, description, eventDate, location, eventTypeId, organiserId)  
                                    values (@eventName, @description, @eventDate, @location, @eventTypeId, @organiserId);";
                    using (SqlCommand add_event_type = new SqlCommand(query, connect))
                    {
                        add_event_type.Parameters.AddWithValue("@eventName", eventName);
                        add_event_type.Parameters.AddWithValue("@description", description);
                        add_event_type.Parameters.AddWithValue("@eventDate", eventDate);
                        add_event_type.Parameters.AddWithValue("@location", location);
                        add_event_type.Parameters.AddWithValue("@eventTypeId", eventTypeId);
                        add_event_type.Parameters.AddWithValue("@organiserId", organiserId);

                        add_event_type.ExecuteNonQuery();
                        return new Event
                        {
                            EventName = eventName,
                            Description = description,
                            EventDate = eventDate.ToString(),
                            Location = location,
                            EventType = eventTypeId.ToString(),
                            Organiser = organiserId.ToString()
                        };
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException("Unable to retrieve the event from the database.", error);
            }
        }

        public Event? EditEvent(int eventId, string eventName, string description, DateTime eventDate, string location, int eventTypeId, int organiserId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"update Events 
                                    set eventName =@eventName, 
                                        description =@description,
                                        eventDate = @eventDate,
                                        location = @location,
                                        eventTypeId = @eventTypeId,
                                        organiserId = @organiserId
                                    where eventId = @eventId;";
                    using (SqlCommand get_event = new SqlCommand(query, connect))
                    {
                        get_event.Parameters.AddWithValue("@eventId", eventId);
                        get_event.Parameters.AddWithValue("@eventName", eventName);
                        get_event.Parameters.AddWithValue("@description", description);
                        get_event.Parameters.AddWithValue("@eventDate", eventDate);
                        get_event.Parameters.AddWithValue("@location", location);
                        get_event.Parameters.AddWithValue("@eventTypeId", eventTypeId);
                        get_event.Parameters.AddWithValue("@organiserId", organiserId);

                        connect.Open();
                        int event_data = get_event.ExecuteNonQuery();
                        if (event_data == 0)
                        {
                            return null;
                        }               
                    }
                }
                return GetEvent(eventId);
            }
            catch (Exception error)
            {
                 throw new InvalidOperationException("Unable to update the event.", error);
            }
        }


        public bool DeleteEvent(int eventId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"delete from Events
                where eventId = @eventId;";

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@eventId", eventId);

                        connect.Open();

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException error) when (error.Number == 547) // Foreign key violation
            {
                throw new InvalidOperationException(
                    "Unable to delete the event because it's referenced by other tables.", error);
            }
        }

    }
}


