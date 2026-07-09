using System;
using System.Collections.Generic;
using System.Text;

namespace DentLink.BusinessLogicLayer.DTOs.Doctordtos
{
    public class CompleteSessionResultDto
    {
        public int SessionId { get; set; }
        public DateTime SessionEnd { get; set; }
        public string Message { get; set; }
    }
}
