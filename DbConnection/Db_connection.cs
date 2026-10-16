using Microsoft.Data.SqlClient;
using RaceDay.Controllers;
using RaceDay.Models;

namespace RaceDay.DbConnection
{
    public class Db_connection
    {
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
                        connect.Close();
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

        //Event types
        public List<EventType> GetEventTypes()
        {
            List<EventType> eventTypes = new List<EventType>();
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"select * from EventTypes;";
                    SqlCommand event_type_data = new SqlCommand(query, connect);
                    connect.Open();
                    SqlDataReader reader = event_type_data.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            eventTypes.Add(new EventType
                            {
                                EventTypeId = (int)reader["eventTypeId"],
                                EventTypeName = reader["eventTypeName"].ToString()!
                            });
                        }
                        connect.Close();
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the event types from the database.", error);
            }
            return eventTypes;
        }

        public EventType? GetEventType(int eventTypeId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"select * from EventTypes where eventTypeId = @eventTypeId;";
                    using (SqlCommand get_event_type = new SqlCommand(query, connect))
                    {
                        get_event_type.Parameters.AddWithValue("@eventTypeId", eventTypeId);
                        connect.Open();
                        SqlDataReader reader = get_event_type.ExecuteReader();
                        if (reader.Read())
                        {
                            return new EventType
                            {
                                EventTypeId = (int)reader["eventTypeId"],
                                EventTypeName = reader["eventTypeName"].ToString()!
                            };
                        }
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the event type from the database.", error);
            }
            return null;
        }

        public EventType? AddEventType(string eventTypeName)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"insert into EventTypes(eventTypeName) values (@eventTypeName);";
                    using (SqlCommand add_event_type = new SqlCommand(query, connect))
                    {
                        add_event_type.Parameters.AddWithValue("@eventTypeName", eventTypeName);
                        connect.Open();
                        add_event_type.ExecuteNonQuery();
                        return new EventType
                        {
                            EventTypeName = eventTypeName
                        };
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to add the event type to the database.", error);
            }
        }

        //Categories
        public List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"select categoryId, categoryName, Categories.eventId, Events.eventName 
                                    from Categories 
                                    inner join Events on Events.eventId = Categories.eventId;";
                    SqlCommand category_data = new SqlCommand(query, connect);
                    connect.Open();
                    SqlDataReader reader = category_data.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                CategoryId = (int)reader["categoryId"],
                                CategoryName = reader["categoryName"].ToString()!,
                                EventName = reader["eventName"].ToString()!
                            });
                        }
                        connect.Close();
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the categories from the database.", error);
            }
            return categories;
        }

        public Category? AddCategory(string categoryName, int eventId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"insert into Categories(categoryName, eventId) values (@categoryName, @eventId);";
                    using (SqlCommand add_category = new SqlCommand(query, connect))
                    {
                        add_category.Parameters.AddWithValue("@categoryName", categoryName);
                        add_category.Parameters.AddWithValue("@eventId", eventId);
                        connect.Open();
                        add_category.ExecuteNonQuery();
                        return new Category
                        {
                            CategoryName = categoryName,
                            EventName = GetEvent(eventId)?.EventName ?? string.Empty
                        };
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to add the category to the database.", error);
            }
        }

        public Category? GetCategory(int categoryId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"select categoryId, categoryName, Categories.eventId, Events.eventName 
                                    from Categories 
                                    inner join Events on Events.eventId = Categories.eventId
                                    where categoryId = @categoryId;";
                    using (SqlCommand get_category = new SqlCommand(query, connect))
                    {
                        get_category.Parameters.AddWithValue("@categoryId", categoryId);
                        connect.Open();
                        SqlDataReader reader = get_category.ExecuteReader();
                        if (reader.Read())
                        {
                            return new Category
                            {
                                CategoryId = (int)reader["categoryId"],
                                CategoryName = reader["categoryName"].ToString()!,
                                EventName = reader["eventName"].ToString()!
                            };
                        }
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the category from the database.", error);
            }
            return null;
        }

        public Category? EditCategory(int categoryId, string categoryName, int eventId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"update Categories 
                                    set categoryName = @categoryName, 
                                        eventId = @eventId
                                    where categoryId = @categoryId;";
                    using (SqlCommand edit_category = new SqlCommand(query, connect))
                    {
                        edit_category.Parameters.AddWithValue("@categoryId", categoryId);
                        edit_category.Parameters.AddWithValue("@categoryName", categoryName);
                        edit_category.Parameters.AddWithValue("@eventId", eventId);
                        connect.Open();
                        int rowsAffected = edit_category.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return null;
                        }
                    }
                }
                return GetCategory(categoryId);
            }
            catch (Exception error)
            {
                throw new InvalidOperationException("Unable to update the category.", error);
            }
        }

        public bool DeleteCategory(int categoryId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"delete from Categories
                                    where categoryId = @categoryId;";
                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@categoryId", categoryId);
                        connect.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        connect.Close();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException error) when (error.Number == 547) // Foreign key violation
            {
                throw new InvalidOperationException(
                    "Unable to delete the category because it's referenced by other tables.", error);
            }
        }

        //Enrolments********************************************************
        public List<Enrolment> GetEnrolments()
        {
            List<Enrolment> enrolments = new List<Enrolment>();
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"select Enrolments.enrolmentId, Enrolments.participantId, Users.firstName + ' ' + Users.lastName as participantName,
                                    Categories.categoryId, Categories.categoryName,  Events.eventId, Events.eventName, Enrolments.enrolmentStatus
                                    from Enrolments
                                    inner join Users on Users.userId = Enrolments.participantId
                                    inner join Categories on Categories.categoryId = Enrolments.categoryId
                                    inner join Events on Events.eventId = Categories.eventId;";
                    SqlCommand enrolment_data = new SqlCommand(query, connect);
                    connect.Open();
                    SqlDataReader reader = enrolment_data.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            enrolments.Add(new Enrolment
                            {
                                EnrolmentId = (int)reader["enrolmentId"],
                                ParticipantId = (int)reader["participantId"],
                                ParticipantName = reader["participantName"].ToString()!,
                                CategoryId = (int)reader["categoryId"],
                                Category = reader["categoryName"].ToString()!,
                                EventId = (int)reader["eventId"],
                                Event = reader["eventName"].ToString()!,
                                EnrolmentStatus = reader["enrolmentStatus"].ToString()!
                            });
                        }
                        connect.Close();
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the enrolments from the database.", error);
            }
            return enrolments;
        }

        public Enrolment? AddEnrolment(int participantId, int eventId, int categoryId, string enrolmentStatus)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"insert into Enrolments(participantId, categoryId,eventId, enrolmentStatus) 
                                    values (@participantId, @categoryId, @eventId, @enrolmentStatus);
                                    select cast(scope_identity() as int);";
                    using (SqlCommand add_enrolment = new SqlCommand(query, connect))
                    {
                        add_enrolment.Parameters.AddWithValue("@participantId", participantId);
                        add_enrolment.Parameters.AddWithValue("@categoryId", categoryId);
                        add_enrolment.Parameters.AddWithValue("@eventId", eventId);
                        add_enrolment.Parameters.AddWithValue("@enrolmentStatus", enrolmentStatus);
                        connect.Open();
                        int newEnrolmentId = (int)add_enrolment.ExecuteScalar()!;
                        return new Enrolment
                        {
                            EnrolmentId = newEnrolmentId,
                            ParticipantId = participantId,
                            ParticipantName = GetUser(participantId)?.FirstName + " " + GetUser(participantId)?.LastName ?? string.Empty,
                            CategoryId = categoryId,
                            Category = GetCategory(categoryId)?.CategoryName ?? string.Empty,
                            EventId = eventId,
                            Event = GetEvent(eventId)?.EventName ?? string.Empty,
                            EnrolmentStatus = enrolmentStatus
                        };
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to add the enrolment to the database.", error);
            }
        }

        public Enrolment? GetEnrolment(int enrolmentId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"select Enrolments.enrolmentId, Enrolments.participantId, Users.firstName + ' ' + Users.lastName as participantName, 
                                            Categories.CategoryId, Categories.categoryName, Events.eventId, Events.eventName, Enrolments.enrolmentStatus
                                    from Enrolments
                                    inner join Users on Users.userId = Enrolments.participantId
                                    inner join Categories on Categories.categoryId = Enrolments.categoryId
                                    inner join Events on Events.eventId = Categories.eventId
                                    where enrolmentId = @enrolmentId;";
                    using (SqlCommand get_enrolment = new SqlCommand(query, connect))
                    {
                        get_enrolment.Parameters.AddWithValue("@enrolmentId", enrolmentId);
                        connect.Open();
                        SqlDataReader reader = get_enrolment.ExecuteReader();
                        if (reader.Read())
                        {
                            return new Enrolment
                            {
                                EnrolmentId = (int)reader["enrolmentId"],
                                ParticipantId = (int)reader["participantId"],
                                ParticipantName = reader["participantName"].ToString()!,
                                CategoryId = (int)reader["categoryId"],
                                Category = reader["categoryName"].ToString()!,
                                EventId = (int)reader["eventId"],
                                Event = reader["eventName"].ToString()!,
                                EnrolmentStatus = reader["enrolmentStatus"].ToString()!
                            };
                        }
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the enrolment from the database.", error);
            }
            return null;
        }

        public Enrolment? EditEnrolment(int enrolmentId, int participantId, int categoryId, int eventId, string enrolmentStatus)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"update Enrolments 
                                    set participantId = @participantId, 
                                        categoryId = @categoryId, 
                                        eventId = @eventId,
                                        enrolmentStatus = @enrolmentStatus
                                    where enrolmentId = @enrolmentId;";
                    using (SqlCommand edit_enrolment = new SqlCommand(query, connect))
                    {
                        edit_enrolment.Parameters.AddWithValue("@enrolmentId", enrolmentId);
                        edit_enrolment.Parameters.AddWithValue("@participantId", participantId);
                        edit_enrolment.Parameters.AddWithValue("@categoryId", categoryId);
                        edit_enrolment.Parameters.AddWithValue("@eventId", eventId);
                        edit_enrolment.Parameters.AddWithValue("@enrolmentStatus", enrolmentStatus);
                        connect.Open();
                        int rowsAffected = edit_enrolment.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return null;
                        }
                    }
                }
                return GetEnrolment(enrolmentId);
            }
            catch (Exception error)
            {
                throw new InvalidOperationException("Unable to update the enrolment.", error);
            }
        }

        public bool DeleteEnrolment(int enrolmentId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"delete from Enrolments
                                    where enrolmentId = @enrolmentId;";
                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@enrolmentId", enrolmentId);
                        connect.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        connect.Close();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException error) when (error.Number == 547) // Foreign key violation
            {
                throw new InvalidOperationException(
                    "Unable to delete the enrolment because it's referenced by other tables.", error);
            }
        }


        //Results**********************************************Results******************************
        public List<Result> GetResults()
        {
            List<Result> results = new List<Result>();

            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"select
                                        Results.resultId,
                                        Results.enrolmentId,
                                        Enrolments.participantId,
                                        Users.firstName + ' ' + Users.lastName AS participantName,
                                        Enrolments.eventId,
                                        Events.eventName,
                                        Results.finishTime,
                                        Results.finishPosition
                                    from Results
                                    inner join Enrolments on Enrolments.enrolmentId = Results.enrolmentId
                                    inner join Users on Users.userId = Enrolments.participantId
                                    left join Events on Events.eventId = Enrolments.eventId;";

                    using (SqlCommand result_data =
                           new SqlCommand(query, connect))
                    {
                        connect.Open();

                        using (SqlDataReader reader = result_data.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Result
                                {
                                    ResultId = (int)reader["resultId"],
                                    EnrolmentId = (int)reader["enrolmentId"],
                                    ParticipantName = reader["participantName"].ToString()!,
                                    EventId = (int)reader["eventId"],
                                    EventName = reader["eventName"].ToString()!,
                                    FinishTime = (TimeSpan)reader["finishTime"],
                                    Position = (int)reader["finishPosition"]
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the results from the database.", error);
            }

            return results;
        }

        public List<Result> GetResultsByEventId(int eventId)
        {
            List<Result> results = new List<Result>();
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"select
                                        Results.resultId,
                                        Results.enrolmentId,
                                        Users.firstName + ' ' + Users.lastName AS participantName,
                                        Events.eventId,
                                        Events.eventName,
                                        Results.finishTime,
                                        Results.finishPosition
                                    from Results
                                    inner join Enrolments on Enrolments.enrolmentId = Results.enrolmentId
                                    inner join Users on Users.userId = Enrolments.participantId
                                    inner join Events on Events.eventId = Enrolments.eventId
                                    where Events.eventId = @eventId;";
                    using (SqlCommand get_result = new SqlCommand(query, connect))
                    {
                        get_result.Parameters.AddWithValue("@eventId", eventId);
                        connect.Open();
                        SqlDataReader reader = get_result.ExecuteReader();
                        while (reader.Read())
                        {
                            results.Add(new Result
                            {
                                ResultId = (int)reader["resultId"],
                                EnrolmentId = (int)reader["enrolmentId"],
                                ParticipantName = reader["participantName"].ToString()!,
                                EventId = (int)reader["eventId"],
                                EventName = reader["eventName"].ToString()!,
                                FinishTime = (TimeSpan)reader["finishTime"],
                                Position = (int)reader["finishPosition"]
                            });
                        }
                        Console.WriteLine($"Results retrieved: {results.Count}");
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the result from the database.", error);
            }
            return results;
        }

        public Result? AddResult(int enrolmentId, TimeSpan finishTime, int position)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = @"insert into Results(enrolmentId, finishTime, finishPosition) 
                                    values (@enrolmentId, @finishTime, @position);
                                    select cast(scope_identity() as int);";
                    using (SqlCommand add_result = new SqlCommand(query, connect))
                    {
                        add_result.Parameters.AddWithValue("@enrolmentId", enrolmentId);
                        add_result.Parameters.AddWithValue("@finishTime", finishTime);
                        add_result.Parameters.AddWithValue("@position", position);
                        connect.Open();
                        int newResultId = (int)add_result.ExecuteScalar()!;
                        return new Result
                        {
                            ResultId = newResultId,
                            EnrolmentId = enrolmentId,
                            ParticipantName = GetUser(GetEnrolment(enrolmentId).ParticipantId).FirstName + " " 
                                            + GetUser(GetEnrolment(enrolmentId).ParticipantId)?.LastName?? string.Empty,
                            
                            EventName = GetEnrolment(enrolmentId)?.Event ?? string.Empty,
                            EventId = GetEnrolment(enrolmentId)?.EventId ?? 0,
                            FinishTime = finishTime,
                            Position = position
                        };
                    }
                }
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to add the result to the database.", error);
            }
        }

        //User profile***************************************User profile******************************
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


