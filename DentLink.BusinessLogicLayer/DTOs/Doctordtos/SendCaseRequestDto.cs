using System;
using System.Collections.Generic;
using System.Text;

namespace DentLink.BusinessLogicLayer.DTOs.Doctordtos
{
    public class SendCaseRequestDto
    {
        public int CaseId { get; set; }
        public decimal TransportCost { get; set; }
    }
}
