using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DentLink.DataAccessLayer.Models
{
    public class CaseRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Case))]
        public int CaseId { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string Status { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TransportCost { get; set; }

        public Case Case { get; set; }
        public Session Session { get; set; }
        public ICollection<SendCaseRequest> SendCaseRequests { get; set; } = new List<SendCaseRequest>();
        public ICollection<SelectCaseRequest> SelectCaseRequests { get; set; } = new List<SelectCaseRequest>();
    }
}
