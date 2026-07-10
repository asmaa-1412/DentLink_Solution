using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace DentLink.DataAccessLayer.Models
{
    public class Session
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(CaseRequest))]
        public int CaseRequestId { get; set; }
        public string Status { get; set; } = "pending";
        public bool PatientArrived { get; set; } = false;

        public DateTime SessionStart { get; set; }

        public DateTime? SessionEnd { get; set; }

        
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }

        // Navigation
        public CaseRequest? CaseRequest { get; set; }
    }
}
