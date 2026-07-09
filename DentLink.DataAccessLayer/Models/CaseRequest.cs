using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DentLink.DataAccessLayer.Enums;

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
        public Status status { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TransportCost { get; set; }

        // --- Navigation Properties ---
        public Case Case { get; set; }

        // ربط One-to-One مع الجلسة (الطلب المقبول بيعمل جلسة)
        public Session Session { get; set; }

        // الربط مع الجداول الوسيطة Many-to-Many
        public ICollection<SendCaseRequest> SendCaseRequests { get; set; } = new List<SendCaseRequest>();
        public ICollection<SelectCaseRequest> SelectCaseRequests { get; set; } = new List<SelectCaseRequest>();
    }
}
