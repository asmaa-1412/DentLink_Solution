using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentLink.DataAccessLayer.Models
{
    public class Case
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }

        [Required]
        public string Type { get; set; } // (Filling, Extraction, Cleaning)

        [Required]
        public string Status { get; set; } // (Available, Taken)

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties ---
        public Patient Patient { get; set; }
        public ICollection<CaseRequest> CaseRequests { get; set; } = new List<CaseRequest>();
    }
}