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
                var enrolments = (
                    from enrolment in _context.Enrolments.AsNoTracking()

                    join user in _context.Users.AsNoTracking()
                        on enrolment.ParticipantId equals user.UserId

                    join category in _context.Categories.AsNoTracking()
                        on enrolment.CategoryId equals category.CategoryId

                    join selectedEvent in _context.Events.AsNoTracking()
                        on enrolment.EventId equals selectedEvent.EventId

                    select new Enrolment
                    {
                        EnrolmentId = enrolment.EnrolmentId,
                        ParticipantId = enrolment.ParticipantId,
                        ParticipantName = user.FirstName + " " + user.LastName,
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        EventId = selectedEvent.EventId,
                        EventName = selectedEvent.EventName,
                        EnrolmentStatus = enrolment.EnrolmentStatus
                    }).ToList();

                return enrolments;
            }
            catch (Exception error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the enrolments from the database.",
                    error);
            }
        }

        public Enrolment? GetEnrolment(int enrolmentId)
        {
            try
            {
                var enrolment = (
                    from e in _context.Enrolments.AsNoTracking()

                    join user in _context.Users.AsNoTracking()
                        on e.ParticipantId equals user.UserId

                    join category in _context.Categories.AsNoTracking()
                        on e.CategoryId equals category.CategoryId

                    join selectedEvent in _context.Events.AsNoTracking()
                        on e.EventId equals selectedEvent.EventId

                    where e.EnrolmentId == enrolmentId

                    select new Enrolment
                    {
                        EnrolmentId = e.EnrolmentId,
                        ParticipantId = e.ParticipantId,
                        ParticipantName = user.FirstName + " " + user.LastName,
                        EventId = selectedEvent.EventId,
                        EventName = selectedEvent.EventName,
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        EnrolmentStatus = e.EnrolmentStatus
                    }).FirstOrDefault();

                return enrolment;
            }
            catch (Exception error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the enrolment from the database.",
                    error);
            }
        }

        public Enrolment? AddEnrolment(int participantId, int eventId, int categoryId, string enrolmentStatus)
        {
            try
            {
                // Check that the participant exists
                var participant = _context.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.UserId == participantId);

                // Check that the category exists
                var category = _context.Categories
                    .AsNoTracking()
                    .FirstOrDefault(c => c.CategoryId == categoryId);

                // Check that the event exists
                var selectedEvent = _context.Events
                    .AsNoTracking()
                    .FirstOrDefault(e => e.EventId == eventId);

                if (participant == null ||
                    category == null ||
                    selectedEvent == null ||
                    category.EventId != eventId)
                {
                    return null;
                }

                var newEnrolment = new Enrolment
                {
                    ParticipantId = participantId,
                    EventId = eventId,
                    CategoryId = categoryId,
                    EnrolmentStatus = enrolmentStatus
                };

                _context.Enrolments.Add(newEnrolment);
                _context.SaveChanges();

                return new Enrolment
                {
                    EnrolmentId = newEnrolment.EnrolmentId,
                    ParticipantId = participantId,
                    ParticipantName = participant.FirstName + " " + participant.LastName,
                    EventId = eventId,
                    EventName = selectedEvent.EventName,
                    CategoryId = categoryId,
                    CategoryName = category.CategoryName,
                    EnrolmentStatus = enrolmentStatus
                };
            }
            catch (DbUpdateException error)
            {
                throw new InvalidOperationException(
                    "Unable to add the enrolment to the database.", error);
            }
        }
        public Enrolment? EditEnrolment(
    int enrolmentId,
    int participantId,
    int categoryId,
    int eventId,
    string enrolmentStatus)
        {
            try
            {
                // Find the enrolment belonging to this participant
                var existingEnrolment = _context.Enrolments
                    .FirstOrDefault(e =>
                        e.EnrolmentId == enrolmentId &&
                        e.ParticipantId == participantId);

                if (existingEnrolment == null)
                {
                    return null;
                }

                // Check that the category belongs to the selected event
                var category = _context.Categories
                    .AsNoTracking()
                    .FirstOrDefault(c =>
                        c.CategoryId == categoryId &&
                        c.EventId == eventId);

                if (category == null)
                {
                    return null;
                }

                // Update the enrolment
                existingEnrolment.CategoryId = categoryId;
                existingEnrolment.EventId = eventId;
                existingEnrolment.EnrolmentStatus = enrolmentStatus;

                _context.SaveChanges();

                return GetEnrolment(enrolmentId);
            }
            catch (DbUpdateException error)
            {
                throw new InvalidOperationException(
                    "Unable to update the enrolment.", error);
            }
        }

        public bool DeleteEnrolment(int enrolmentId)
        {
            try
            {
                var existingEnrolment = _context.Enrolments
                    .FirstOrDefault(e => e.EnrolmentId == enrolmentId);

                if (existingEnrolment == null)
                {
                    return false;
                }

                _context.Enrolments.Remove(existingEnrolment);

                _context.SaveChanges();

                return true;
            }
            catch (DbUpdateException error)
            {
                throw new InvalidOperationException(
                    "Unable to delete the enrolment because it is referenced by other records.",
                    error);
            }
        }

        public List<Enrolment> GetEnrolmentsForOrganiser(int organiserId)
        {
            try
            {
                var enrolments = (
                    from enrolment in _context.Enrolments.AsNoTracking()

                    join user in _context.Users.AsNoTracking()
                        on enrolment.ParticipantId equals user.UserId

                    join category in _context.Categories.AsNoTracking()
                        on enrolment.CategoryId equals category.CategoryId

                    join selectedEvent in _context.Events.AsNoTracking()
                        on enrolment.EventId equals selectedEvent.EventId

                    where selectedEvent.OrganiserId == organiserId

                    select new Enrolment
                    {
                        EnrolmentId = enrolment.EnrolmentId,
                        ParticipantId = enrolment.ParticipantId,
                        ParticipantName = user.FirstName + " " + user.LastName,
                        EventId = selectedEvent.EventId,
                        EventName = selectedEvent.EventName,
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        EnrolmentStatus = enrolment.EnrolmentStatus
                    }).ToList();

                return enrolments;
            }
            catch (Exception error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the organiser's enrolments.",
                    error);
            }
        }

        public bool OrganiserOwnsEnrolment(int enrolmentId, int organiserId)
        {
            return (
                from enrolment in _context.Enrolments.AsNoTracking()

                join selectedEvent in _context.Events.AsNoTracking()
                    on enrolment.EventId equals selectedEvent.EventId

                where enrolment.EnrolmentId == enrolmentId
                      && selectedEvent.OrganiserId == organiserId

                select enrolment
            ).Any();
        }

        public bool OrganiserOwnsEvent(int eventId, int organiserId)
        {
            return _context.Events
                .AsNoTracking()
                .Any(e => e.EventId == eventId &&
                          e.OrganiserId == organiserId);
        }
    }
}