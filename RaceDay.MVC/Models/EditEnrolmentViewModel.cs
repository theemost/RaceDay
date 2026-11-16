namespace RaceDay.MVC.Models
{
    public class EditEnrolmentViewModel
    {
        public int EnrolmentId { get; set; }

        public int EventId { get; set; }

        public int CategoryId { get; set; }

        public string EnrolmentStatus { get; set; } = string.Empty;
    }
}