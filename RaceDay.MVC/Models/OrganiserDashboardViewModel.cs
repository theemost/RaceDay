namespace RaceDay.MVC.Models
{
    public class OrganiserDashboardViewModel
    {
        public List<Event> Events { get; set; } = new();

        public List<Category> Categories { get; set; } = new();

        public List<Enrolment> Enrolments { get; set; } = new();

        public List<Result> Results { get; set; } = new();

        public int TotalEvents => Events.Count;

        public int TotalCategories => Categories.Count;

        public int TotalEnrolments => Enrolments.Count;

        public int ApprovedCount =>
            Enrolments.Count(e =>
                e.EnrolmentStatus.Equals(
                    "Approved",
                    StringComparison.OrdinalIgnoreCase));

        public int PendingCount =>
            Enrolments.Count(e =>
                e.EnrolmentStatus.Equals(
                    "Pending",
                    StringComparison.OrdinalIgnoreCase));

        public int RejectedCount =>
            Enrolments.Count(e =>
                e.EnrolmentStatus.Equals(
                    "Rejected",
                    StringComparison.OrdinalIgnoreCase));

        public int TotalResults => Results.Count;
    }
}