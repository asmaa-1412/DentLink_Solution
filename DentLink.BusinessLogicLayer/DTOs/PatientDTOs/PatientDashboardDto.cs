
using DentLink.DataAccessLayer.Models;

namespace DentLink.BusinessLogicLayer.DTOs.PatientDTOs
{
    public class PatientDashboardDto
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }

        public int MyCasesCount { get; set; }
        public int ActiveCasesCount { get; set; }
        public int CompletedCasesCount { get; set; }

        public int PendingRequestsCount { get; set; }

        public int CompletedSessionsCount { get; set; }
        public DateTime? LastCompletedSessionDate { get; set; }
        public string ImageUrl { get; set; } 

        public IEnumerable<Case> RecentCases { get; set; }
    }
}

