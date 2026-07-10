using DentLink.BusinessLogicLayer.DTOs.PatientDTOs;
using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Data;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DentLink.PresentationLayer.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PatientsController(IUnitOfWork unitOfWork, AppDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var patient = await _unitOfWork.Repository<Patient>()
                .GetEntityWithSpec(p => p.Id == id, p => p.User);

            if (patient == null) return NotFound();

            ViewBag.PatientId = patient.Id;
            ViewBag.PatientName = patient.User?.FullName;
            ViewBag.PatientImageUrl = patient.ImageUrl; 

            return View("~/Views/Patient/profile.cshtml", patient);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(int id, [FromForm] UpdateProfileDTO model)
        {
            if (model == null) return BadRequest("Data Is null!");

            var patient = await _unitOfWork.Repository<Patient>()
                .GetEntityWithSpec(p => p.Id == id, p => p.User);

            if (patient == null) return NotFound();

            ModelState.Remove("Email");

            if (!ModelState.IsValid)
            {
                ViewBag.PatientId = patient.Id;
                ViewBag.PatientName = patient.User?.FullName;
                ViewBag.PatientImageUrl = patient.ImageUrl;
                return View("~/Views/Patient/profile.cshtml", patient);
            }

            patient.Address = model.Location;

            if (patient.User != null)
            {
                patient.User.FullName = model.FullName;
                patient.User.PhoneNumber = model.Phone;

                _context.Users.Update(patient.User);
            }

            if (model.ImageUrl != null && model.ImageUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "profiles");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{id}_{DateTime.Now.Ticks}{Path.GetExtension(model.ImageUrl.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageUrl.CopyToAsync(fileStream);
                }

                patient.ImageUrl = $"/assets/images/profiles/{uniqueFileName}";
            }

            _unitOfWork.Repository<Patient>().Update(patient);
            await _unitOfWork.CompleteAsync();

            if (patient.User != null)
            {
                await _signInManager.RefreshSignInAsync(patient.User);
                var claims = new List<Claim>
                {
                    new Claim("FullName", patient.User.FullName ?? ""),
                    new Claim("ImageUrl", patient.ImageUrl ?? "")
                };
                await _userManager.AddClaimsAsync(patient.User, claims);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";

            return RedirectToAction("Profile", new { id = patient.Id });
        }
    }
}