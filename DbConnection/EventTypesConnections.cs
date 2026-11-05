using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.DbConnection
{
    public class EventTypesConnection
    {

        private readonly RaceDayContext _context;

        public EventTypesConnection(RaceDayContext context)
        {
            _context = context;
        }

        //Event types
        public List<EventType> GetEventTypes()
        {
            List<EventType> eventTypes = new List<EventType>();
            try
            {
                eventTypes = (from event_type in _context.EventTypes.AsNoTracking()
                              select event_type).ToList();
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
                var eventType = from event_type in _context.EventTypes.AsNoTracking()
                                where event_type.EventTypeId == eventTypeId
                                select event_type;
    
                return eventType.FirstOrDefault();
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the event type from the database.", error);
            }
        }

        public EventType? AddEventType(string eventTypeName)
        {

            try
            {
                var newEventType = new EventType
                {
                    EventTypeName = eventTypeName
                };
                _context.EventTypes.Add(newEventType);
                _context.SaveChanges();

                return newEventType;
            }
            catch (SqlException error)
            {
                throw new InvalidOperationException(
                    "Unable to add the event type to the database.", error);
            }
        }


    }
}