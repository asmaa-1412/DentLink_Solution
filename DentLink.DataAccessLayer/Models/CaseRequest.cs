
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentLink.DataAccessLayer.Models
{
    public class CaseRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Case))]
        public int CaseId { get; set; }

        [Required]
        public string Status { get; set; } = "pending"; // (pending, accepted, rejected)

        [Required]
        public decimal TransportCost { get; set; } // تكلفة المواصلات المحددة في الطلب

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties ---
        public Case Case { get; set; }

        // ربط One-to-One مع الجلسة (الطلب المقبول بيعمل جلسة)
        public Session Session { get; set; }

        // الربط مع الجداول الوسيطة Many-to-Many
        public ICollection<SendCaseRequest> SendCaseRequests { get; set; } = new List<SendCaseRequest>();
        public ICollection<SelectCaseRequest> SelectCaseRequests { get; set; } = new List<SelectCaseRequest>();
    }
}