
using DentLink.BusinessLogicLayer.DTOs.PatientDTOs;
using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DentLink.PresentationLayer.Controllers
{
    [Route("[controller]")]
    public class CasesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CasesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        [HttpPost("create")]
        public async Task<IActionResult> CreateCase([FromForm] CreateCaseDto caseDto)
        {
            if (caseDto == null) return BadRequest(new { Message = "Data Not Valid!" });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            string savedImageUrl = null;

            if (caseDto.DentalImage != null && caseDto.DentalImage.Length > 0)
            {
                // Location
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "cases");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{caseDto.PatientId}_{Guid.NewGuid()}_{Path.GetFileName(caseDto.DentalImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await caseDto.DentalImage.CopyToAsync(fileStream);
                }

                savedImageUrl = $"/images/cases/{uniqueFileName}";
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

            return Json(new { Message = "Case created successfully!", CaseId = newCase.Id });
        }



        [HttpGet("MyCases/{patientId}")]
        public async Task<IActionResult> GetPatientCases(int patientId)
        {
            var cases = await _unitOfWork.Repository<Case>().FindAsync(c => c.PatientId == patientId);

            return Json(cases);
        }
    }
}
    