namespace DentLink.DTOs.Doctordtos
{
    public class AvailableCaseDto
    {
        public int CaseId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string PatientName { get; set; }
        public string PatientAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool AlreadyRequested { get; set; }
    }
}
