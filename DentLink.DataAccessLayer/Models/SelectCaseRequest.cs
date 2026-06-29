using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DentLink.DataAccessLayer.Models
{
    public class SelectCaseRequest
    {
        [Required]
        [ForeignKey(nameof(CaseRequest))]
        public int CaseRequestId { get; set; }

        [Required]
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }

        // Navigation
        public CaseRequest CaseRequest { get; set; }
        public Patient Patient { get; set; }
    }

}
