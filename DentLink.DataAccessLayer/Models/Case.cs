using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DentLink.DataAccessLayer.Enums;
using Microsoft.AspNetCore.Http;

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
        public Typies Typies { get; set; }

        [Required]
        public Status status { get; set; }

        public string Description { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Patient Patient { get; set; }
        public ICollection<CaseRequest> CaseRequests { get; set; } = new List<CaseRequest>();
    }
}
