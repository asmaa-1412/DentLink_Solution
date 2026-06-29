using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentLink.DataAccessLayer.Models
{
    public class Session
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(CaseRequest))]
        public int CaseRequestId { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "confirmed"; // (confirmed, in progress, completed)

        public bool IsPatientArrived { get; set; } = false; // بيتحول True لما تدوسي "Confirm Patient Arrived"
        public bool IsCompleted { get; set; } = false;      // بيتحول True لما تدوسي "Complete Session"

        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }

        // --- Navigation Properties ---
        public CaseRequest CaseRequest { get; set; }
    }
}