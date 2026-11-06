using Microsoft.Data.SqlClient;
using RaceDay.Models;
using RaceDay.Data;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.DbConnection
{
    public class ResultsConnection
    {

        private readonly RaceDayContext _context;

        public ResultsConnection(RaceDayContext context)
        {
            _context = context;
        }

        //Results**********************************************Results******************************
        public List<Result> GetResults()
        {
            try
            {
                List<Result> results = new List<Result>();

                results = (from result in _context.Results.AsNoTracking()
                           join enrolment in _context.Enrolments.AsNoTracking()
                           on result.EnrolmentId equals enrolment.EnrolmentId
                           join user in _context.Users.AsNoTracking()
                           on enrolment.ParticipantId equals user.UserId
                           join category in _context.Categories.AsNoTracking()
                           on enrolment.CategoryId equals category.CategoryId
                           join event_ in _context.Events.AsNoTracking()
                           on category.EventId equals event_.EventId
                           select new Result
                           {
                               ResultId = result.ResultId,
                               EnrolmentId = result.EnrolmentId,
                               ParticipantName = user.FirstName + " " + user.LastName,
                               EventId = event_.EventId,
                               EventName = event_.EventName,
                               FinishTime = result.FinishTime,
                               Position = result.Position
                               }).ToList();
                
                return results;
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the results from the database.", error);
            }
        }

        /*public List<Result> GetResultsByEventId(int eventId)
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
        }*/
    }
}


