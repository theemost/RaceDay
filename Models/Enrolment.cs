namespace RaceDay.Models
{
    public class Enrolment
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public int EventId { get; set; }
        public string Event { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string EnrolmentStatus { get; set; } = string.Empty;

    }
}
