using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DentLink.DataAccessLayer.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Address { get; set; }

        public int Age { get; set; }

        public string? ImageUrl { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }

        // Navigation
        public ICollection<Case> Cases { get; set; } = new List<Case>();
        public ICollection<SelectCaseRequest> SelectCaseRequests { get; set; } = new List<SelectCaseRequest>();
    }
}
