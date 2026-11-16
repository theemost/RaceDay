namespace RaceDay.Models
{
    public class EditEventRequest
    {
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int EventTypeId { get; set; }
    }
}