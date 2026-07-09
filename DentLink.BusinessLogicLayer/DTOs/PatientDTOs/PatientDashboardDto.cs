
namespace DentLink.BusinessLogicLayer.DTOs.PatientDTOs
{
    public class PatientDashboardDto
    {
        public int MyCasesCount { get; set; }
        public int PendingRequestsCount { get; set; }
        public int CompletedSessionsCount { get; set; }

        public IEnumerable<object> RecentCases { get; set; }
    }
}
