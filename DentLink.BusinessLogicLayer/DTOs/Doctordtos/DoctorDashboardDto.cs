namespace DentLink.DTOs.Doctordtos
{
    public class DoctorDashboardDto
    {
        public int AvailableCasesCount { get; set; }
        public int MatchingSpecializationCount { get; set; }
        public int AcceptedRequestsCount { get; set; }
        public int UpcomingSessionsCount { get; set; }
        public int CompletedSessionsCount { get; set; }
        public DateTime? LastCompletedSessionDate { get; set; }

        public IEnumerable<RecentActivityDto> RecentActivity { get; set; }
        public IEnumerable<UpcomingSessionDto> UpcomingSessions { get; set; }
    }
}
