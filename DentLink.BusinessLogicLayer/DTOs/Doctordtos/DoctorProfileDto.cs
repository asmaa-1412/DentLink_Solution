namespace DentLink.DTOs.Doctordtos
{
    public class DoctorProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string University { get; set; }
        public string Faculty { get; set; }
        public string Phone { get; set; }
        public string AcademicYear { get; set; }
        public string IdCardUrl { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}
