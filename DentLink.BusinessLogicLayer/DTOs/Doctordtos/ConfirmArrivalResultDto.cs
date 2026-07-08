using System;
using System.Collections.Generic;
using System.Text;

namespace DentLink.BusinessLogicLayer.DTOs.Doctordtos
{
    public class ConfirmArrivalResultDto
    {
        public int SessionId { get; set; }
        public bool PatientArrived { get; set; }
        public string Message { get; set; }
    }
}
