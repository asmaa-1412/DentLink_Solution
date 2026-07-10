using System;
using System.Collections.Generic;
using System.Text;

namespace DentLink.BusinessLogicLayer.DTOs.AdminDTOs
{
    public class PendingDoctorSummaryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string University { get; set; }
        public string AcademicYear { get; set; }
    }
}
