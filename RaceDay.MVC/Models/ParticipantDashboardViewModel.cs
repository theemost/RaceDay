namespace RaceDay.MVC.Models
{
    public class ParticipantDashboardViewModel
    {
        public List<Enrolment> Enrolments { get; set; }
            = new List<Enrolment>();

        public List<Result> Results { get; set; }
            = new List<Result>();

        public int TotalEvents =>
            Enrolments.Select(enrolment => enrolment.EventId).Distinct().Count();

        public int TotalEnrolments => Enrolments.Count;

        public int ApprovedCount =>
            Enrolments.Count(enrolment =>
                enrolment.EnrolmentStatus.Equals(
                    "Approved",
                    StringComparison.OrdinalIgnoreCase));

        public int PendingCount =>
            Enrolments.Count(enrolment =>
                enrolment.EnrolmentStatus.Equals(
                    "Pending",
                    StringComparison.OrdinalIgnoreCase));

        public int RejectedCount =>
            Enrolments.Count(enrolment =>
                enrolment.EnrolmentStatus.Equals(
                    "Rejected",
                    StringComparison.OrdinalIgnoreCase));

        public int EnrolledCount =>
            Enrolments.Count(enrolment =>
                enrolment.EnrolmentStatus.Equals(
                    "Enrolled",
                    StringComparison.OrdinalIgnoreCase));

        public int TotalResults => Results.Count;
    }
}