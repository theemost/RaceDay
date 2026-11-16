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

        public List<Result> GetResults()
        {
            try
            {
                var results = (
                    from result in _context.Results.AsNoTracking()

                    join enrolment in _context.Enrolments.AsNoTracking()
                        on result.EnrolmentId equals enrolment.EnrolmentId

                    join user in _context.Users.AsNoTracking()
                        on enrolment.ParticipantId equals user.UserId

                    join _event in _context.Events.AsNoTracking()
                        on enrolment.EventId equals _event.EventId
                    join category in _context.Categories.AsNoTracking()
                        on enrolment.CategoryId equals category.CategoryId
                    select new Result
                    {
                        ResultId = result.ResultId,
                        EnrolmentId = result.EnrolmentId,
                        ParticipantName = user.FirstName + " " + user.LastName,
                        EventId = _event.EventId,
                        EventName = _event.EventName,
                        CategoryName = category.CategoryName,
                        FinishTime = result.FinishTime, 
                        Position = result.Position
                    }).ToList();

                return results;
            }
            catch (Exception error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the results from the database.",
                    error);
            }
        }

        public List<Result> GetResultsByEventId(int eventId)
        {
            try
            {
                var results = (
                    from result in _context.Results.AsNoTracking()

                    join enrolment in _context.Enrolments.AsNoTracking()
                        on result.EnrolmentId equals enrolment.EnrolmentId

                    join user in _context.Users.AsNoTracking()
                        on enrolment.ParticipantId equals user.UserId

                    join selectedEvent in _context.Events.AsNoTracking()
                        on enrolment.EventId equals selectedEvent.EventId

                    join category in _context.Categories.AsNoTracking()
                        on enrolment.CategoryId equals category.CategoryId

                    where selectedEvent.EventId == eventId

                    select new Result
                    {
                        ResultId = result.ResultId,
                        EnrolmentId = result.EnrolmentId,
                        ParticipantName = user.FirstName + " " + user.LastName,
                        EventId = selectedEvent.EventId,
                        EventName = selectedEvent.EventName,
                        CategoryName = category.CategoryName,
                        FinishTime = result.FinishTime,
                        Position = result.Position
                    }).ToList();

                return results;
            }
            catch (Exception error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve results for this event.",
                    error);
            }
        }

        public Result? AddResult(
    int enrolmentId,
    TimeSpan finishTime)
        {
            try
            {
                // Find the enrolment
                var enrolment = _context.Enrolments
                    .AsNoTracking()
                    .FirstOrDefault(e => e.EnrolmentId == enrolmentId);

                if (enrolment == null)
                {
                    return null;
                }

                // Retrieve participant
                var participant = _context.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.UserId == enrolment.ParticipantId);

                // Retrieve event
                var selectedEvent = _context.Events
                    .AsNoTracking()
                    .FirstOrDefault(e => e.EventId == enrolment.EventId);

                if (participant == null || selectedEvent == null)
                {
                    return null;
                }

                if (finishTime < TimeSpan.Zero)
                {
                    throw new ArgumentException("Invalid finishing time.");
                }

                using var transaction = _context.Database.BeginTransaction();

                // Create the result
                var newResult = new Result
                {
                    EnrolmentId = enrolmentId,
                    FinishTime = finishTime,
                    Position = 0
                };

                _context.Results.Add(newResult);
                _context.SaveChanges();

                // Find all enrolments in the same event and category
                var enrolmentIds = _context.Enrolments
                    .Where(e =>
                        e.EventId == enrolment.EventId &&
                        e.CategoryId == enrolment.CategoryId)
                    .Select(e => e.EnrolmentId);

                // Retrieve and sort all results by finishing time
                var categoryResults = _context.Results
                    .Where(r => enrolmentIds.Contains(r.EnrolmentId))
                    .OrderBy(r => r.FinishTime)
                    .ThenBy(r => r.ResultId)
                    .ToList();

                // Automatically calculate positions
                for (int i = 0; i < categoryResults.Count; i++)
                {
                    if (i == 0 ||
                        categoryResults[i].FinishTime !=
                        categoryResults[i - 1].FinishTime)
                    {
                        categoryResults[i].Position = i + 1;
                    }
                    else
                    {
                        categoryResults[i].Position =
                            categoryResults[i - 1].Position;
                    }
                }

                // Save updated positions
                _context.SaveChanges();

                transaction.Commit();

                return new Result
                {
                    ResultId = newResult.ResultId,
                    EnrolmentId = enrolmentId,
                    ParticipantName = participant.FirstName + " " + participant.LastName,
                    EventId = selectedEvent.EventId,
                    EventName = selectedEvent.EventName,
                    FinishTime = finishTime,
                    Position = newResult.Position
                };
            }
            catch (DbUpdateException error)
            {
                throw new InvalidOperationException(
                    "Unable to add the result to the database.", error);
            }
        }

        // Get results belonging to an organiser's events
        public List<Result> GetResultsForOrganiser(int organiserId)
        {
            var results = (
                from result in _context.Results.AsNoTracking()

                join enrolment in _context.Enrolments.AsNoTracking()
                    on result.EnrolmentId equals enrolment.EnrolmentId

                join user in _context.Users.AsNoTracking()
                    on enrolment.ParticipantId equals user.UserId

                join selectedEvent in _context.Events.AsNoTracking()
                    on enrolment.EventId equals selectedEvent.EventId

                join category in _context.Categories.AsNoTracking()
                    on enrolment.CategoryId equals category.CategoryId

                where selectedEvent.OrganiserId == organiserId

                select new Result
                {
                    ResultId = result.ResultId,
                    EnrolmentId = result.EnrolmentId,
                    ParticipantName = user.FirstName + " " + user.LastName,
                    EventId = selectedEvent.EventId,
                    EventName = selectedEvent.EventName,
                    CategoryName = category.CategoryName,
                    FinishTime = result.FinishTime,
                    Position = result.Position
                }).ToList();

            return results;
        }

        // Get results belonging to a participant
        public List<Result> GetResultsForParticipant(int participantId)
        {
            var results = (
                from result in _context.Results.AsNoTracking()

                join enrolment in _context.Enrolments.AsNoTracking()
                    on result.EnrolmentId equals enrolment.EnrolmentId

                join user in _context.Users.AsNoTracking()
                    on enrolment.ParticipantId equals user.UserId

                join selectedEvent in _context.Events.AsNoTracking()
                    on enrolment.EventId equals selectedEvent.EventId

                join category in _context.Categories.AsNoTracking()
                    on enrolment.CategoryId equals category.CategoryId

                where enrolment.ParticipantId == participantId

                select new Result
                {
                    ResultId = result.ResultId,
                    EnrolmentId = result.EnrolmentId,
                    ParticipantName = user.FirstName + " " + user.LastName,
                    EventId = selectedEvent.EventId,
                    EventName = selectedEvent.EventName,
                    CategoryName = category.CategoryName,
                    FinishTime = result.FinishTime,
                    Position = result.Position
                }).ToList();

            return results;
        }

        // Check whether an organiser owns an event
        public bool OrganiserOwnsEvent(int eventId, int organiserId)
        {
            return _context.Events
                .AsNoTracking()
                .Any(e => e.EventId == eventId &&
                          e.OrganiserId == organiserId);
        }

        // Check whether an enrolment belongs to an organiser's event
        public bool OrganiserOwnsEnrolment(int enrolmentId, int organiserId)
        {
            return (
                from enrolment in _context.Enrolments.AsNoTracking()

                join selectedEvent in _context.Events.AsNoTracking()
                    on enrolment.EventId equals selectedEvent.EventId

                where enrolment.EnrolmentId == enrolmentId &&
                      selectedEvent.OrganiserId == organiserId

                select enrolment
            ).Any();
        }

        // Recalculate all existing results for an organiser's events
        // Recalculate existing result positions for an organiser
        public void RecalculatePositionsForOrganiser(int organiserId)
        {
            try
            {
                using var transaction = _context.Database.BeginTransaction();

                var results = (
                    from result in _context.Results

                    join enrolment in _context.Enrolments
                        on result.EnrolmentId equals enrolment.EnrolmentId

                    join selectedEvent in _context.Events
                        on enrolment.EventId equals selectedEvent.EventId

                    where selectedEvent.OrganiserId == organiserId

                    select new
                    {
                        Result = result,
                        EventId = enrolment.EventId,
                        CategoryId = enrolment.CategoryId
                    }).ToList();

                // Group results by event and category
                var groups = results.GroupBy(r => new
                {
                    r.EventId,
                    r.CategoryId
                });

                foreach (var group in groups)
                {
                    var orderedResults = group
                        .OrderBy(r => r.Result.FinishTime)
                        .ThenBy(r => r.Result.ResultId)
                        .ToList();

                    int position = 0;

                    for (int i = 0; i < orderedResults.Count; i++)
                    {
                        if (i == 0 ||
                            orderedResults[i].Result.FinishTime !=
                            orderedResults[i - 1].Result.FinishTime)
                        {
                            position = i + 1;
                        }

                        orderedResults[i].Result.Position = position;
                    }
                }

                _context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception error)
            {
                throw new InvalidOperationException(
                    "Unable to recalculate result positions.", error);
            }
        }
    }
}


