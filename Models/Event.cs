namespace RaceDay.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EventDate { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Organiser { get; set; } = string.Empty;

    }
}
