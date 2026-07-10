using DentLink.BusinessLogicLayer.DTOs.PatientDTOs;
using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Data;
using DentLink.DataAccessLayer.Enums;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentLink.PresentionLayer.Controllers
{
    [Authorize(Roles = "Patient")]
    public class RequestsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;

        public RequestsController(IUnitOfWork unitOfWork, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

       
        [HttpGet]
        public async Task<IActionResult> MyRequests(int patientId)
        {
            if (patientId <= 0) return BadRequest("Invalid patient.");

            var requests = await _context.CaseRequests
                .Include(cr => cr.Case)
                .Include(cr => cr.SendCaseRequests)
                    .ThenInclude(sc => sc.Doctor)
                        .ThenInclude(d => d.User)
                .Where(cr => cr.Case.PatientId == patientId)
                .OrderByDescending(cr => cr.RequestedAt)
                .ToListAsync();

            var dto = requests.SelectMany(cr => cr.SendCaseRequests.Select(sc => new PatientCaseRequestDto
            {
                RequestId = cr.Id,
                CaseId = cr.CaseId,
                CaseType = cr.Case?.Typies.ToString(),
                CaseDescription = cr.Case?.Description,
                DoctorId = sc.DoctorId,
                DoctorName = sc.Doctor?.User?.FullName ?? "Unknown",
                University = sc.Doctor?.University,
                Department = sc.Doctor?.Department,
                AcademicYear = sc.Doctor?.AcademicYear,
                TransportCost = cr.TransportCost,
                RequestedAt = cr.RequestedAt,
                Status = cr.status.ToString()
            })).ToList();

            ViewBag.PatientId = patientId;
            return View("~/Views/Patient/requests.cshtml", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptRequest(int requestId, int patientId)
        {
            var request = await _unitOfWork.Repository<CaseRequest>().GetByIdAsync(requestId);
            if (request == null) return NotFound();

            if (request.status == Status.Active)
            {
                TempData["ErrorMessage"] = "This request is already accepted.";
                return RedirectToAction(nameof(MyRequests), new { patientId });
            }

            request.status = Status.Active;

            var currentCase = await _unitOfWork.Repository<Case>().GetByIdAsync(request.CaseId);
            if (currentCase != null)
                currentCase.status = Status.Active;

            var newSession = new Session
            {
                CaseRequestId = request.Id,
                SessionStart = DateTime.UtcNow,
                PatientArrived = false
            };
            await _unitOfWork.Repository<Session>().AddAsync(newSession);

            
            var otherRequests = await _unitOfWork.Repository<CaseRequest>()
                .FindAsync(cr => cr.CaseId == request.CaseId && cr.Id != request.Id && cr.status == Status.Pending);

            foreach (var other in otherRequests)
                other.status = Status.Cancelled;

            await _unitOfWork.CompleteAsync();

            TempData["SuccessMessage"] = "Request accepted! The student will be notified and will contact you shortly.";
            return RedirectToAction(nameof(MyRequests), new { patientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int requestId, int patientId)
        {
            var request = await _unitOfWork.Repository<CaseRequest>().GetByIdAsync(requestId);
            if (request == null) return NotFound();

            if (request.status == Status.Active)
            {
                TempData["ErrorMessage"] = "Cannot reject an already accepted request.";
                return RedirectToAction(nameof(MyRequests), new { patientId });
            }

            request.status = Status.Cancelled;
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(MyRequests), new { patientId });
        }
    }
}