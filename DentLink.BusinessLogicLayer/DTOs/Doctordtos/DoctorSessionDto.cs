namespace DentLink.DTOs.Doctordtos
{
    public class DoctorSessionDto
    {
        public int SessionId { get; set; }
        public int CaseRequestId { get; set; }
        public string PatientName { get; set; }
        public string CaseType { get; set; }
        public DateTime SessionStart { get; set; }
        public DateTime? SessionEnd { get; set; }
        public bool PatientArrived { get; set; }
        public string Status { get; set; }  
    }
}
