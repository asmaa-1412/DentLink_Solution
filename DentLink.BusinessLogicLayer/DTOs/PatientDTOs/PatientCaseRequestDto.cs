using System;
using System.Collections.Generic;
using System.Text;

namespace DentLink.BusinessLogicLayer.DTOs.PatientDTOs
{
    public class PatientCaseRequestDto
    {
        public int RequestId { get; set; }        
        public int CaseId { get; set; }
        public string CaseType { get; set; }
        public string CaseDescription { get; set; }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string University { get; set; }
        public string Department { get; set; }
        public string AcademicYear { get; set; }

        public decimal TransportCost { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; }
    }
}
