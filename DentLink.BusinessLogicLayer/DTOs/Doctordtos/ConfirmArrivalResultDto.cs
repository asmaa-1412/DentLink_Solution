namespace DentLink.DTOs.Doctordtos
{
    public class ConfirmArrivalResultDto
    {
        public int SessionId { get; set; }
        public bool PatientArrived { get; set; }
        public string Message { get; set; }
    }
}
