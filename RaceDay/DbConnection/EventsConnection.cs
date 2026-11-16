using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.DbConnection
{
    public class EventsConnection
    {
        private readonly RaceDayContext _context;

        public EventsConnection(RaceDayContext context)
        {
            _context = context;
        }

        private string connection = @"Data source=(localdb)\RaceDay;Database=RaceDay;";

        //EVENTS****EVENTS
        public List<Event> GetEvents()
        {
            List<Event> events = new List<Event>();
            try
            {
                events = (from _event in _context.Events.AsNoTracking()
                          join event_type in _context.EventTypes.AsNoTracking()
                          on _event.EventTypeId equals event_type.EventTypeId
                          join _user in _context.Users.AsNoTracking()
                          on _event.OrganiserId equals _user.UserId
                          select new Event
                          {
                              EventId = _event.EventId,
                              EventName = _event.EventName,
                              Description = _event.Description,
                              EventDate = _event.EventDate,
                              Location = _event.Location,
                              EventTypeId = _event.EventTypeId,
                              EventType = event_type.EventTypeName,
                              OrganiserId = _event.OrganiserId,
                              Organiser = _user.FirstName + " " + _user.LastName
                          }).ToList();
            }
            catch (SqlException error)
            {
                // Log the error appropriately in a real application.
                throw new InvalidOperationException(
                    "Unable to retrieve events from the database.", error);
            }
            return events;
        }

        public Event? GetEvent(int eventId)
        {//GetEvent
            try
            {//Try
                var eventDetails =
                        (from events in _context.Events.AsNoTracking()
                        join eventTypes in _context.EventTypes.AsNoTracking()
                            on events.EventTypeId equals eventTypes.EventTypeId
                        join users in _context.Users.AsNoTracking()
                            on events.OrganiserId equals users.UserId
                        where events.EventId == eventId
                        select new Event
                        {
                            EventId = events.EventId,
                            EventName = events.EventName,
                            Description = events.Description,
                            EventDate = events.EventDate,
                            Location = events.Location,
                            EventTypeId = events.EventTypeId,
                            OrganiserId = events.OrganiserId,

                            // Get names from the related tables
                            EventType = eventTypes.EventTypeName,
                            Organiser = users.FirstName + " " + users.LastName
                        });

                return eventDetails.FirstOrDefault();
            }//Try
            catch (SqlException error)
            {
                // Log the error appropriately in a real application.
                throw new InvalidOperationException(
                    "Unable to retrieve the event from the database.", error);
            }
        }

        public Event? AddEvent(string eventName, string description, DateTime eventDate, string location, int eventTypeId, int organiserId)
        {
            try
            {
                var newEvent = new Event
                {
                    EventName = eventName,
                    Description = description,
                    EventDate = eventDate,
                    Location = location,
                    EventTypeId = eventTypeId,
                    OrganiserId = organiserId
                };

                _context.Events.Add(newEvent);

                _context.SaveChanges();

                return newEvent;
            }
            catch (Exception error)
            {
                throw new InvalidOperationException(
                    "Unable to add the event to the database.",
                    error);
            }
        }

        public Event? EditEvent(int eventId, string eventName, string description, DateTime eventDate, string location, int eventTypeId, int organiserId)
        {
            try
            {
                var existingEvent = _context.Events
                    .FirstOrDefault(e => e.EventId == eventId);

                if (existingEvent == null ||
                    existingEvent.OrganiserId != organiserId)
                {
                    return null;
                }

                existingEvent.EventName = eventName;
                existingEvent.Description = description;
                existingEvent.EventDate = eventDate;
                existingEvent.Location = location;
                existingEvent.EventTypeId = eventTypeId;

                _context.SaveChanges();

                return GetEvent(eventId);
            }
            catch (Exception error)
            {
                throw new InvalidOperationException(
                    "Unable to update the event.", error);
            }
        }

        public bool DeleteEvent(int eventId, int organiserId)
        {
            try
            {
                var existingEvent = _context.Events
                    .FirstOrDefault(e => e.EventId == eventId);

                if (existingEvent == null ||
                    existingEvent.OrganiserId != organiserId)
                {
                    return false;
                }

                _context.Events.Remove(existingEvent);

                _context.SaveChanges();

                return true;
            }
            catch (DbUpdateException error)
            {
                throw new InvalidOperationException(
                    "Unable to delete the event because it's referenced by other tables.",
                    error);
            }
        }
    }
}