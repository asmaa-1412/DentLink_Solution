using DentLink.BusinessLogicLayer.DTOs.Doctordtos;
using DentLink.DataAccessLayer.Data;
using DentLink.DataAccessLayer.Enums;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentLink.PresentionLayer.Controllers
{
    [Authorize(Roles = "Student")]
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<Doctor> GetCurrentDoctorAsync()
        {
            var userId = _userManager.GetUserId(User);
            return await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.UserId == userId);
        }

       
        public async Task<IActionResult> Dashboard()
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return NotFound();

            if (!doctor.IsApproved)
                return RedirectToAction("PendingApproval", "Account");

            var availableCasesCount = await _context.Cases.CountAsync(c => c.status == Status.Pending);

            var acceptedRequestsCount = await _context.SendCaseRequests
                .CountAsync(s => s.DoctorId == doctor.Id && s.CaseRequest.status == Status.Active);

            var upcomingSessions = await _context.Sessions
                .Include(s => s.CaseRequest).ThenInclude(cr => cr.Case).ThenInclude(c => c.Patient).ThenInclude(p => p.User)
                .Where(s => s.CaseRequest.SendCaseRequests.Any(sc => sc.DoctorId == doctor.Id) && s.SessionEnd == null)
                .OrderBy(s => s.SessionStart)
                .ToListAsync();

            var completedSessions = await _context.Sessions
                .Include(s => s.CaseRequest)
                .Where(s => s.CaseRequest.SendCaseRequests.Any(sc => sc.DoctorId == doctor.Id) && s.SessionEnd != null)
                .OrderByDescending(s => s.SessionEnd)
                .ToListAsync();

            var recentRequests = await _context.SendCaseRequests
                .Include(s => s.CaseRequest).ThenInclude(cr => cr.Case).ThenInclude(c => c.Patient).ThenInclude(p => p.User)
                .Where(s => s.DoctorId == doctor.Id)
                .OrderByDescending(s => s.CaseRequest.RequestedAt)
                .Take(5)
                .ToListAsync();

            var recentActivity = recentRequests.Select(r =>
            {
                string statusText = r.CaseRequest.status switch
                {
                    Status.Active => "was accepted",
                    Status.Cancelled => "was rejected",
                    _ => "is pending review"
                };
                return new RecentActivityDto
                {
                    Message = $"Your request for {r.CaseRequest.Case?.Patient?.User?.FullName ?? "a patient"} {statusText}",
                    Timestamp = r.CaseRequest.RequestedAt
                };
            }).ToList();

            var dashboardData = new DoctorDashboardDto
            {
                AvailableCasesCount = availableCasesCount,
                MatchingSpecializationCount = availableCasesCount,
                AcceptedRequestsCount = acceptedRequestsCount,
                UpcomingSessionsCount = upcomingSessions.Count,
                CompletedSessionsCount = completedSessions.Count,
                LastCompletedSessionDate = completedSessions.FirstOrDefault()?.SessionEnd,
                RecentActivity = recentActivity,
                UpcomingSessions = upcomingSessions.Take(3).Select(s => new UpcomingSessionDto
                {
                    SessionId = s.Id,
                    PatientName = s.CaseRequest?.Case?.Patient?.User?.FullName ?? "Unknown",
                    CaseType = s.CaseRequest?.Case?.Typies.ToString(),
                    SessionStart = s.SessionStart,
                    Status = s.PatientArrived ? "in-progress" : "confirmed"
                }).ToList()
            };

            ViewBag.DoctorName = doctor.User?.FullName;
            return View("~/Views/student/student-dashboard.cshtml", dashboardData);
        }

        public async Task<IActionResult> AvailableCases(string searchQuery, string selectedType)
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return NotFound();
            if (!doctor.IsApproved) return RedirectToAction("PendingApproval", "Account");

            var casesQuery = _context.Cases
                .Include(c => c.Patient).ThenInclude(p => p.User)
                .Where(c => c.status == Status.Pending)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                string s = searchQuery.ToLower().Trim();
                casesQuery = casesQuery.Where(c =>
                    c.Typies.ToString().ToLower().Contains(s) ||
                    c.Description.ToLower().Contains(s));
            }

            if (!string.IsNullOrEmpty(selectedType) && selectedType != "All")
            {
                if (Enum.TryParse<Typies>(selectedType, out var typeEnum))
                    casesQuery = casesQuery.Where(c => c.Typies == typeEnum);
            }

            var cases = await casesQuery.OrderByDescending(c => c.CreatedAt).ToListAsync();

            var requestedCaseIds = await _context.SendCaseRequests
                .Where(s => s.DoctorId == doctor.Id)
                .Select(s => s.CaseRequest.CaseId)
                .ToListAsync();

            var dto = cases.Select(c => new AvailableCaseDto
            {
                CaseId = c.Id,
                Type = c.Typies.ToString(),
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                PatientName = c.Patient?.User?.FullName ?? "Unknown",
                PatientAddress = c.Patient?.Address,
                CreatedAt = c.CreatedAt,
                AlreadyRequested = requestedCaseIds.Contains(c.Id)
            }).ToList();

            ViewBag.DoctorName = doctor.User?.FullName;
            return View("~/Views/student/available-cases.cshtml", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendRequest(int caseId, decimal transportCost)
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return NotFound();
            if (!doctor.IsApproved) return RedirectToAction("PendingApproval", "Account");

            bool alreadyRequested = await _context.SendCaseRequests
                .AnyAsync(s => s.DoctorId == doctor.Id && s.CaseRequest.CaseId == caseId);

            if (alreadyRequested)
            {
                TempData["ErrorMessage"] = "You already sent a request for this case.";
                return RedirectToAction(nameof(AvailableCases));
            }

            var targetCase = await _context.Cases.FindAsync(caseId);
            if (targetCase == null || targetCase.status != Status.Pending)
            {
                TempData["ErrorMessage"] = "This case is no longer available.";
                return RedirectToAction(nameof(AvailableCases));
            }

            var newRequest = new CaseRequest
            {
                CaseId = caseId,
                status = Status.Pending,
                TransportCost = transportCost,
                RequestedAt = DateTime.UtcNow
            };
            _context.CaseRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            _context.SendCaseRequests.Add(new SendCaseRequest
            {
                DoctorId = doctor.Id,
                CaseRequestId = newRequest.Id
            });
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your request has been sent successfully!";
            return RedirectToAction(nameof(MyRequests));
        }

        public async Task<IActionResult> MyRequests()
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return NotFound();

            var myRequests = await _context.SendCaseRequests
                .Include(s => s.CaseRequest).ThenInclude(cr => cr.Case).ThenInclude(c => c.Patient).ThenInclude(p => p.User)
                .Where(s => s.DoctorId == doctor.Id)
                .OrderByDescending(s => s.CaseRequest.RequestedAt)
                .Select(s => new MyRequestDto
                {
                    CaseRequestId = s.CaseRequestId,
                    CaseId = s.CaseRequest.CaseId,
                    CaseType = s.CaseRequest.Case.Typies.ToString(),
                    PatientName = s.CaseRequest.Case.Patient.User.FullName,
                    PatientAddress = s.CaseRequest.Case.Patient.Address,
                    TransportCost = s.CaseRequest.TransportCost,
                    Status = s.CaseRequest.status.ToString(),
                    RequestedAt = s.CaseRequest.RequestedAt
                })
                .ToListAsync();

            ViewBag.DoctorName = doctor.User?.FullName;
            return View("~/Views/student/my-requests.cshtml", myRequests);
        }

        public async Task<IActionResult> Sessions()
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return NotFound();

            var sessions = await _context.Sessions
                .Include(s => s.CaseRequest).ThenInclude(cr => cr.Case).ThenInclude(c => c.Patient).ThenInclude(p => p.User)
                .Where(s => s.CaseRequest.SendCaseRequests.Any(sc => sc.DoctorId == doctor.Id))
                .OrderByDescending(s => s.SessionStart)
                .Select(s => new DoctorSessionDto
                {
                    SessionId = s.Id,
                    CaseRequestId = s.CaseRequestId,
                    PatientName = s.CaseRequest.Case.Patient.User.FullName,
                    CaseType = s.CaseRequest.Case.Typies.ToString(),
                    SessionStart = s.SessionStart,
                    SessionEnd = s.SessionEnd,
                    PatientArrived = s.PatientArrived,
                    Status = s.SessionEnd != null ? "completed" : (s.PatientArrived ? "in progress" : "confirmed")
                })
                .ToListAsync();

            ViewBag.DoctorName = doctor.User?.FullName;
            return View("~/Views/student/sessions.cshtml", sessions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmArrival(int sessionId)
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return NotFound();

            var session = await _context.Sessions
                .Include(s => s.CaseRequest).ThenInclude(cr => cr.SendCaseRequests)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null || !session.CaseRequest.SendCaseRequests.Any(sc => sc.DoctorId == doctor.Id))
                return NotFound();

            session.PatientArrived = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Sessions));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteSession(int sessionId)
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return NotFound();

            var session = await _context.Sessions
                .Include(s => s.CaseRequest).ThenInclude(cr => cr.SendCaseRequests)
                .Include(s => s.CaseRequest.Case)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null || !session.CaseRequest.SendCaseRequests.Any(sc => sc.DoctorId == doctor.Id))
                return NotFound();

            session.SessionEnd = DateTime.UtcNow;

            if (session.CaseRequest.Case != null)
                session.CaseRequest.Case.status = Status.Completed;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Sessions));
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return NotFound();

            var dto = new DoctorProfileDto
            {
                Id = doctor.Id,
                FullName = doctor.User?.FullName,
                University = doctor.University,
                Faculty = doctor.Department,
                Phone = doctor.User?.PhoneNumber,
                AcademicYear = doctor.AcademicYear,
                IdCardUrl = doctor.IdCardUrl,
                IsApproved = doctor.IsApproved,
                ApprovalDate = doctor.ApprovalDate
            };

            ViewBag.ProfilePicture = doctor.ProfilePicture;
            ViewBag.Email = doctor.User?.Email;

            return View("~/Views/student/profile.cshtml", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateDoctorProfileDto model, IFormFile? ProfilePictureFile)
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return NotFound();

            if (ProfilePictureFile != null && ProfilePictureFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "profiles");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{doctor.Id}_{Guid.NewGuid()}_{Path.GetFileName(ProfilePictureFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePictureFile.CopyToAsync(fileStream);
                }

                doctor.ProfilePicture = $"/assets/images/profiles/{uniqueFileName}";
            }

            if (doctor.User != null)
            {
                doctor.User.FullName = model.FullName;
                doctor.User.PhoneNumber = model.Phone;
            }
            doctor.University = model.University;
            doctor.Department = model.Faculty;
            doctor.AcademicYear = model.AcademicYear;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }
    }
}