using System.ComponentModel.DataAnnotations;
namespace DentLink.PresentionLayer.Models
{
    public class PatientRegisterViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Range(1, 120)]
        public int Age { get; set; }

        [Required]
        public string Governorate { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required, MinLength(8)]
        public string Password { get; set; }

        [Required, Compare("Password", ErrorMessage = "الباسورد مش متطابق")]
        public string ConfirmPassword { get; set; }
    }
    
}
