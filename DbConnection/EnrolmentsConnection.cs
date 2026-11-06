using Microsoft.Data.SqlClient;
using RaceDay.Models;
using RaceDay.Data;
using Microsoft.EntityFrameworkCore;    

namespace RaceDay.DbConnection
{
    public class EnrolmentsConnection
    {

        private readonly RaceDayContext _context;

        public EnrolmentsConnection(RaceDayContext context)
        {
            _context = context;
        }

        //Enrolments********************************************************
        public List<Enrolment> GetEnrolments()
        {
            try
            {
                List<Enrolment> enrolments = new List<Enrolment>();
                
                enrolments = (from enrolment in _context.Enrolments.AsNoTracking()
                              join user in _context.Users.AsNoTracking()
                              on enrolment.ParticipantId equals user.UserId
                              join category in _context.Categories.AsNoTracking()
                              on enrolment.CategoryId equals category.CategoryId
                              join event_ in _context.Events.AsNoTracking()
                              on category.EventId equals event_.EventId
                              select new Enrolment
                              {
                                  EnrolmentId = enrolment.EnrolmentId,
                                  ParticipantId = enrolment.ParticipantId,
                                  ParticipantName = user.FirstName + " " + user.LastName,
                                  CategoryId = enrolment.CategoryId,
                                  Category = category.CategoryName,
                                  EventId = category.EventId,
                                  Event = event_.EventName,
                                  EnrolmentStatus = enrolment.EnrolmentStatus
                              }).ToList();

                return enrolments;
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the enrolments from the database.", error);
            }
        }
        public Enrolment? GetEnrolment(int enrolmentId)
        {
            try
            {
                var enrolmentDetails = from enrolment in _context.Enrolments.AsNoTracking()
                                       join user in _context.Users.AsNoTracking()
                                       on enrolment.ParticipantId equals user.UserId
                                       join category in _context.Categories.AsNoTracking()
                                       on enrolment.CategoryId equals category.CategoryId
                                       join event_ in _context.Events.AsNoTracking()
                                       on category.EventId equals event_.EventId
                                       where enrolment.EnrolmentId == enrolmentId
                                       select new Enrolment
                                       {
                                           EnrolmentId = enrolment.EnrolmentId,
                                           ParticipantId = enrolment.ParticipantId,
                                           ParticipantName = user.FirstName + " " + user.LastName,
                                           CategoryId = enrolment.CategoryId,
                                           Category = category.CategoryName,
                                           EventId = category.EventId,
                                           Event = event_.EventName,
                                           EnrolmentStatus = enrolment.EnrolmentStatus
                                       };
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the enrolment from the database.", error);
            }
            return null;
        }

        /*public Enrolment? AddEnrolment(int participantId, int eventId, int categoryId, string enrolmentStatus)
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
        }*/
    }
}
