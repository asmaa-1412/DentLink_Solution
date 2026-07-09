using DentLink.BusinessLogicLayer.DTOs.PatientDTOs;
using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DentLink.PresentationLayer.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id) 
        {
            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(id);
            if (patient == null) return NotFound();

            return View(patient);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(int id, [FromForm] UpdateProfileDTO model)
        {
            if (model == null) return BadRequest("Data Is null!");

            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(id);
            if (patient == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(patient);
            }

            patient.FullName = model.FullName;
            patient.Address = model.Location;

            if (model.ImageUrl != null && model.ImageUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "profiles");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{id}_{Path.GetFileName(model.ImageUrl.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageUrl.CopyToAsync(fileStream);
                }

                patient.ImageUrl = $"/assets/images/profiles/{uniqueFileName}";
            }

            _unitOfWork.Repository<Patient>().Update(patient);
            await _unitOfWork.CompleteAsync();

            TempData["SuccessMessage"] = "Profile updated successfully!";

            return RedirectToAction("Profile", new { id = patient.Id });
        }
    }
}