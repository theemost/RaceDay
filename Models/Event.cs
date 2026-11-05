namespace RaceDay.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Organiser { get; set; } = string.Empty;
        public int EventTypeId { get; set; }
        public int OrganiserId { get; set; }
    }
}
