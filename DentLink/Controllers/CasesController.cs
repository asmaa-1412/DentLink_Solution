using DentLink.BusinessLogicLayer.DTOs.PatientDTOs;
using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DentLink.PresentationLayer.Controllers
{
    public class CasesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CasesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int patientId)
        {
            if (patientId <= 0) return BadRequest("Invalid patient.");

            var patient = await _unitOfWork.Repository<Patient>()
                .GetEntityWithSpec(p => p.Id == patientId, p => p.User);

            ViewBag.PatientId = patientId;
            ViewBag.PatientName = patient?.User?.FullName;
            ViewBag.PatientImageUrl = patient?.ImageUrl;

            ViewBag.CaseTypes = Enum.GetValues(typeof(DataAccessLayer.Enums.Typies))
                .Cast<DentLink.DataAccessLayer.Enums.Typies>();

            return View("~/Views/Patient/Create.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCaseDto caseDto)
        {
            if (caseDto == null) return BadRequest("Data Not Valid!");

            if (!ModelState.IsValid)
            {
                var patient = await _unitOfWork.Repository<Patient>()
                    .GetEntityWithSpec(p => p.Id == caseDto.PatientId, p => p.User);

                ViewBag.PatientId = caseDto.PatientId;
                ViewBag.PatientName = patient?.User?.FullName;
                ViewBag.PatientImageUrl = patient?.ImageUrl;

                ViewBag.CaseTypes = Enum.GetValues(typeof(DataAccessLayer.Enums.Typies))
                    .Cast<DentLink.DataAccessLayer.Enums.Typies>();

                return View("~/Views/Patient/Create.cshtml", caseDto);
            }

            string savedImageUrl = null;

            if (caseDto.DentalImage != null && caseDto.DentalImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "cases");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{caseDto.PatientId}_{Guid.NewGuid()}_{Path.GetFileName(caseDto.DentalImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await caseDto.DentalImage.CopyToAsync(fileStream);
                }

                savedImageUrl = $"/assets/images/cases/{uniqueFileName}";
            }

            var newCase = new Case
            {
                PatientId = caseDto.PatientId,
                Typies = caseDto.CaseType,
                status = DataAccessLayer.Enums.Status.Pending,
                Description = caseDto.Description,
                ImageUrl = savedImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Case>().AddAsync(newCase);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("MyCases", new { id = caseDto.PatientId });
        }

        [HttpGet]
        public async Task<IActionResult> MyCases(int id)
        {
            var cases = await _unitOfWork.Repository<Case>().FindAsync(c => c.PatientId == id);
            var casesList = (cases ?? Enumerable.Empty<Case>()).OrderByDescending(c => c.Id).ToList();

            var patient = await _unitOfWork.Repository<Patient>()
                .GetEntityWithSpec(p => p.Id == id, p => p.User);

            ViewBag.PatientId = id;
            ViewBag.PatientName = patient?.User?.FullName;
            ViewBag.PatientImageUrl = patient?.ImageUrl;

            return View("~/Views/Patient/my-cases.cshtml", casesList);
        }
    }
}