using System;
using System.Collections.Generic;
using System.Text;

namespace DentLink.BusinessLogicLayer.DTOs.Doctordtos
{
    public class DoctorProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string University { get; set; }
        public string Faculty { get; set; }
        public string Phone { get; set; }
        public string AcademicYear { get; set; }
        public string IdCardUrl { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}
