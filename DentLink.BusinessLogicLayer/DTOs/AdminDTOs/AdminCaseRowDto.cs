using System;
using System.Collections.Generic;
using System.Text;

namespace DentLink.BusinessLogicLayer.DTOs.AdminDTOs
{
    public class AdminCaseRowDto
    {
        public int CaseId { get; set; }
        public string CaseType { get; set; }
        public string PatientName { get; set; }
        public string StudentName { get; set; }
        public DentLink.DataAccessLayer.Enums.Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
