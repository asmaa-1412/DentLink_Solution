using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DentLink.DataAccessLayer.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string University { get; set; }

        [Required]
        [StringLength(100)]
        public string Faculty { get; set; }

        [Required]
        [StringLength(20)]
        public string AcademicYear { get; set; } // (4th Year مثلاً)

        [Required]
        [StringLength(50)]
        public string StudentIDNumber { get; set; } // الـ ID الجامعي

        public string? ProfilePicture { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsApproved { get; set; } = false;

        // --- Navigation Properties ---
        // الربط مع الجدول الوسيط للطلبات اللي الدكتور بيبعتها
        public ICollection<SendCaseRequest> SendCaseRequests { get; set; } = new List<SendCaseRequest>();
    }
}