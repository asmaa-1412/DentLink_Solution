
using Microsoft.AspNetCore.Http;

namespace DentLink.BusinessLogicLayer.DTOs.PatientDTOs
{
    public class UpdateProfileDTO
    {
        
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Location { get; set; }

            public IFormFile? ImageUrl { get; set; }
        }
    }
