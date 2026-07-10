using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DentLink.PresentionLayer.Models
{
    public class StudentRegisterViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string University { get; set; }

        [Required]
        public string College { get; set; }

        [Required]
        public string AcademicYear { get; set; }

        [Required]
        public IFormFile StudentId { get; set; }   // صورة/PDF الكارنيه

        [Required, MinLength(8)]
        public string Password { get; set; }

        [Required, Compare("Password", ErrorMessage = "Password is not identical")]
        public string ConfirmPassword { get; set; }
    }
}