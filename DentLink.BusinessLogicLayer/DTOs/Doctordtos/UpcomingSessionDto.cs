namespace DentLink.DTOs.Doctordtos
{
    public class UpcomingSessionDto
    {
        public int SessionId { get; set; }
        public string PatientName { get; set; }
        public string CaseType { get; set; }
        public DateTime SessionStart { get; set; }
        public string Status { get; set; } 
    }
}
