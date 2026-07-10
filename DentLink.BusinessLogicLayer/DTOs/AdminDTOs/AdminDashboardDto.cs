using System;
using System.Collections.Generic;
using System.Text;

namespace DentLink.BusinessLogicLayer.DTOs.AdminDTOs
{
    public class AdminDashboardDto
    {
        public int TotalDoctorsCount { get; set; }
        public int PendingDoctorsCount { get; set; }
        public int TotalPatientsCount { get; set; }
        public int TotalCasesCount { get; set; }

        public List<PendingDoctorSummaryDto> PendingDoctors { get; set; } = new();
        public List<AdminCaseRowDto> RecentCases { get; set; } = new();
    }
}
