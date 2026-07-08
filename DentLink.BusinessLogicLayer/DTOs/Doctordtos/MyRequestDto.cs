using System;
using System.Collections.Generic;
using System.Text;

namespace DentLink.BusinessLogicLayer.DTOs.Doctordtos
{
    public class MyRequestDto
    {
        public int CaseRequestId { get; set; }
        public int CaseId { get; set; }
        public string CaseType { get; set; }
        public string PatientName { get; set; }
        public string PatientAddress { get; set; }
        public decimal TransportCost { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
