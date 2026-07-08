using DentLink.BusinessLogicLayer.DTOs.PatientDTOs;
using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DentLink.PresentationLayer.Controllers
{
    
    [Route("[controller]")]
    public class PatientsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        [HttpGet("Profile/{patientId}")]
        public async Task<IActionResult> GetPatientProfile(int patientId)
        {
            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(patientId);
            if (patient == null) return NotFound(new { Message = "Patient Not Found." });

            var profileData = new
            {
                patient.Id,
                patient.FullName,
                patient.Email ,
                patient.Phone ,
                Location = patient.Address,
                patient.ImageUrl 
            };

            return Json(profileData);
        }



        [HttpPost("Profile/{patientId}")]
        public async Task<IActionResult> UpdatePatientProfile(int patientId, [FromForm] UpdateProfileDTO model)
        {
            if (model == null) return BadRequest(new { Message = "Data Is null!" });

            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(patientId);
            if (patient == null) return NotFound(new { Message = "This Patient Not Found." });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            patient.FullName = model.FullName;
            patient.Address = model.Location;

            if (model.ImageUrl != null && model.ImageUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{patientId}_{Path.GetFileName(model.ImageUrl.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageUrl.CopyToAsync(fileStream);
                }

                patient.ImageUrl = $"/images/profiles/{uniqueFileName}";
            }

            _unitOfWork.Repository<Patient>().Update(patient);
            await _unitOfWork.CompleteAsync();

            var updatedData = new
            {
                Message = "Profile updated successfully!",
                Profile = new
                {
                    patient.Id,
                    patient.FullName,
                    Location = patient.Address,
                    patient.ImageUrl
                }
            };

            return Json(updatedData);
        }
    }
}
