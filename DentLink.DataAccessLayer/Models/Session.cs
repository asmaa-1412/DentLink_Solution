using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DentLink.DataAccessLayer.Models;

namespace DentLink.DataAccessLayer.Models
{
    public class Session
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(CaseRequest))]
        public int CaseRequestId { get; set; }

        public DateTime SessionStart { get; set; }

        public DateTime? SessionEnd { get; set; }

        public bool PatientArrived { get; set; } = false;

        // Navigation
        public CaseRequest CaseRequest { get; set; }
    }
}
