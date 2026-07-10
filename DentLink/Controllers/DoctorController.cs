using DentLink.DataAccessLayer.Data;
using DentLink.DataAccessLayer.Enums;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentLink.PresentionLayer.Controllers
{
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager= userManager;
        }

        // ==========================================
        // 1. Dashboard Action
        // ==========================================
        public async Task<IActionResult> Dashboard()
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == 1);

            ViewBag.DoctorName = doctor?.User.FullName ?? "Omar Khaled";
            ViewBag.IsApproved = doctor?.IsApproved ?? false;

            // "Available" = Pending حسب الـ enum عندنا
            ViewBag.AvailableCasesCount = await _context.Cases.CountAsync(c => c.status == Status.Pending);
            // "Accepted" = Active حسب الـ enum عندنا
            ViewBag.AcceptedRequestsCount = await _context.CaseRequests.CountAsync(cr => cr.status == Status.Active);
            ViewBag.CompletedSessionsCount = await _context.Sessions.CountAsync(s => s.Status == "completed");

            var recentRequests = await _context.SendCaseRequests
                .Include(s => s.CaseRequest)
                    .ThenInclude(cr => cr.Case)
                .OrderByDescending(s => s.CaseRequest.RequestedAt)
                .Take(3)
                .ToListAsync();
            ViewBag.RecentRequests = recentRequests;

            var upcomingSessions = await _context.Sessions
                .Include(s => s.CaseRequest)
                    .ThenInclude(cr => cr.Case)
                        .ThenInclude(c => c.Patient)
                .Where(s => s.Status != "completed")
                .OrderBy(s => s.SessionStart) // كان SessionDate وهي مش موجودة
                .Take(3)
                .ToListAsync();

            return View(upcomingSessions);
        }

        // ==========================================
        // 2. Under Review Action
        // ==========================================
        public IActionResult UnderReview()
        {
            return View();
        }

        // ==========================================
        // 3. Available Cases Action
        // ==========================================
        public async Task<IActionResult> AvailableCases(string searchQuery, string selectedType)
        {
            var casesQuery = _context.Cases
                .Include(c => c.Patient)
                .Where(c => c.status == Status.Pending)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                string searchLower = searchQuery.ToLower().Trim();

                casesQuery = casesQuery.Where(c =>
                    c.Typies.ToString().ToLower().Contains(searchLower) ||
                    c.Description.ToLower().Contains(searchLower)
                );
            }

            if (!string.IsNullOrEmpty(selectedType) && selectedType != "All")
            {
                if (Enum.TryParse<Typies>(selectedType, out var typeEnum))
                {
                    casesQuery = casesQuery.Where(c => c.Typies == typeEnum);
                }
            }

            var availableCases = await casesQuery
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.CurrentSearch = searchQuery;
            ViewBag.CurrentType = string.IsNullOrEmpty(selectedType) ? "All" : selectedType;

            return View(availableCases);
        }

        // --- أكشن حساس: يحتاج موافقة الأدمن ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendRequest(int caseId, decimal transportCost)
        {
            int currentDoctorId = 1;

            var doctor = await _context.Doctors.FindAsync(currentDoctorId);
            if (doctor == null || !doctor.IsApproved)
            {
                return RedirectToAction(nameof(UnderReview));
            }

            bool alreadyRequested = await _context.SendCaseRequests
                .AnyAsync(s => s.DoctorId == currentDoctorId && s.CaseRequest.CaseId == caseId);

            if (alreadyRequested)
            {
                return RedirectToAction(nameof(AvailableCases));
            }

            var newRequest = new CaseRequest
            {
                CaseId = caseId,
                status = Status.Pending, // كانت "pending" (string)
                TransportCost = transportCost,
                RequestedAt = DateTime.UtcNow
            };
            _context.CaseRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            var doctorRequestLink = new SendCaseRequest
            {
                DoctorId = currentDoctorId,
                CaseRequestId = newRequest.Id
            };
            _context.SendCaseRequests.Add(doctorRequestLink);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AvailableCases));
        }

        // ==========================================
        // 4. Sessions Action
        // ==========================================
        public async Task<IActionResult> Sessions()
        {
            int currentDoctorId = 1;

            var doctorSessions = await _context.Sessions
                .Include(s => s.CaseRequest)
                    .ThenInclude(cr => cr.Case)
                        .ThenInclude(c => c.Patient)
                .Where(session => session.CaseRequest.SendCaseRequests.Any(s => s.DoctorId == currentDoctorId))
                .OrderByDescending(session => session.SessionStart) // كان session.st (خطأ إملائي)
                .ToListAsync();

            return View(doctorSessions);
        }

        // --- أكشن حساس: يحتاج موافقة الأدمن ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmArrival(int sessionId)
        {
            int currentDoctorId = 1;

            var doctor = await _context.Doctors.FindAsync(currentDoctorId);
            if (doctor == null || !doctor.IsApproved)
            {
                return RedirectToAction(nameof(UnderReview));
            }

            var session = await _context.Sessions.FindAsync(sessionId);
            if (session != null)
            {
                session.PatientArrived = true; // كانت IsPatientArrived (مش موجودة)
                session.Status = "in progress";
                session.ActualStartTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Sessions));
        }

        // --- أكشن حساس: يحتاج موافقة الأدمن ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteSession(int sessionId)
        {
            int currentDoctorId = 1;

            var doctor = await _context.Doctors.FindAsync(currentDoctorId);
            if (doctor == null || !doctor.IsApproved)
            {
                return RedirectToAction(nameof(UnderReview));
            }

            var session = await _context.Sessions.FindAsync(sessionId);
            if (session != null)
            {
                session.Status = "completed"; // شالينا IsCompleted (مش موجودة)
                session.ActualEndTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Sessions));
        }

        // ==========================================
        // 5. Profile Actions
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == 1);

            if (doctor == null)
            {
                
                var user = new ApplicationUser
                {
                    FullName = "Omar Khaled",
                    Email = "omar.k@cu.edu.eg",
                    UserName = "omar.k@cu.edu.eg" 
                };

                var result = await _userManager.CreateAsync(user, "P@ssw0rd123"); 

                if (!result.Succeeded)
                {
                    
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View();
                }
                
                doctor = new Doctor
                {
                    UserId = user.Id,
                    University = "Cairo University"
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
            }

            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(Doctor updatedDoctor, IFormFile? ProfilePictureFile)
        {
            var doctorInDb = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == updatedDoctor.Id);
            if (doctorInDb == null) return NotFound();

            if (ProfilePictureFile != null && ProfilePictureFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ProfilePictureFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePictureFile.CopyToAsync(fileStream);
                }

                doctorInDb.ProfilePicture = "/images/" + uniqueFileName;
            }

            doctorInDb.User.FullName = updatedDoctor.User.FullName;
            doctorInDb.University = updatedDoctor.University;
            doctorInDb.Department = updatedDoctor.Department;
            doctorInDb.AcademicYear = updatedDoctor.AcademicYear;
            doctorInDb.IdCardUrl = updatedDoctor.IdCardUrl;

            _context.Doctors.Update(doctorInDb);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Profile));
        }
    }
}