
using DentLink.DataAccessLayer.Enums;
using Microsoft.AspNetCore.Http;

namespace DentLink.BusinessLogicLayer.DTOs.PatientDTOs
{
    public class CreateCaseDto
    {
        public int PatientId { get; set; }
        public Typies CaseType { get; set; } 
        public string Description { get; set; }

        public IFormFile? DentalImage { get; set; }
    }
}

