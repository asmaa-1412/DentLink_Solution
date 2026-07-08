using System.ComponentModel.DataAnnotations;

namespace DentLink.DataAccessLayer.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string University { get; set; }

        public string Department { get; set; }

        public string Phone { get; set; }

        public string AcademicYear { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public bool IsApproved { get; set; } = false;

        public string? IdCardUrl { get; set; }

        // Navigation
        public ICollection<SendCaseRequest> SendCaseRequests { get; set; } = new List<SendCaseRequest>();
    }
}
