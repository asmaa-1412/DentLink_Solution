using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentLink.DataAccessLayer.Models
{
    public class SendCaseRequest
    {
        [Required]
        [ForeignKey(nameof(Doctor))]
        public int DoctorId { get; set; }

        [Required]
        [ForeignKey(nameof(CaseRequest))]
        public int CaseRequestId { get; set; }

        public Doctor Doctor { get; set; }
        public CaseRequest CaseRequest { get; set; }
    }
}