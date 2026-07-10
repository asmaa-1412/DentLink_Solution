using DentLink.BusinessLogicLayer.DTOs.AdminDTOs;
using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Data;
using DentLink.DataAccessLayer.Enums;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentLink.PresentionLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDoctorRepository _doctorRepository;

        public AdminController(AppDbContext context, IDoctorRepository doctorRepository)
        {
            _context = context;
            _doctorRepository = doctorRepository;
        }

        // ==========================================
        // Dashboard
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var pendingDoctorsEntities = (await _doctorRepository.GetPendingApprovalsAsync())
                .Take(3)
                .Select(d => new PendingDoctorSummaryDto
                {
                    Id = d.Id,
                    FullName = d.User?.FullName ?? "Unknown",
                    University = d.University,
                    AcademicYear = d.AcademicYear
                })
                .ToList();

            if (pendingDoctorsEntities.Any(d => d.FullName == "Unknown"))
            {
                var ids = pendingDoctorsEntities.Select(d => d.Id).ToList();
                var withUsers = await _context.Doctors
                    .Include(d => d.User)
                    .Where(d => ids.Contains(d.Id))
                    .OrderBy(d => d.User.FullName)
                    .ToListAsync();

                pendingDoctorsEntities = withUsers.Select(d => new PendingDoctorSummaryDto
                {
                    Id = d.Id,
                    FullName = d.User?.FullName ?? "Unknown",
                    University = d.University,
                    AcademicYear = d.AcademicYear
                }).ToList();
            }

            var recentCases = await GetRecentCasesAsync(3);

            var dto = new AdminDashboardDto
            {
                TotalDoctorsCount = await _context.Doctors.CountAsync(),
                PendingDoctorsCount = await _context.Doctors.CountAsync(d => !d.IsApproved),
                TotalPatientsCount = await _context.Patients.CountAsync(),
                TotalCasesCount = await _context.Cases.CountAsync(),
                PendingDoctors = pendingDoctorsEntities,
                RecentCases = recentCases
            };

            return View("~/Views/admin/admin-dashboard.cshtml", dto);
        }

        [HttpGet]
        public async Task<IActionResult> DoctorApprovals()
        {
            var pendingDoctors = await _context.Doctors
                .Include(d => d.User)
                .Where(d => !d.IsApproved)
                .ToListAsync();

            return View("~/Views/admin/doctor-approvals.cshtml", pendingDoctors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            doctor.IsApproved = true;
            doctor.IsVerified = true;
            doctor.ApprovalDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Doctor approved successfully.";
            return RedirectToAction(nameof(DoctorApprovals));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectDoctor(int id)
        {
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == id);
            if (doctor == null) return NotFound();

            if (doctor.User != null)
                _context.Users.Remove(doctor.User);

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Doctor request rejected and removed.";
            return RedirectToAction(nameof(DoctorApprovals));
        }

       
    
        [HttpGet]
        public async Task<IActionResult> AdminDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.IsApproved)
                .ToListAsync();

            return View("~/Views/admin/admin-doctors.cshtml", doctors);
        }

       
        [HttpGet]
        public async Task<IActionResult> AdminPatients()
        {
            var patients = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Cases)
                .ToListAsync();

            return View("~/Views/admin/admin-patients.cshtml", patients);
        }

       
        [HttpGet]
        public async Task<IActionResult> AdminCases()
        {
            var cases = await GetRecentCasesAsync(null);
            return View("~/Views/admin/admin-cases.cshtml", cases);
        }

        
        private async Task<List<AdminCaseRowDto>> GetRecentCasesAsync(int? take)
        {
            var query = _context.Cases
                .Include(c => c.Patient).ThenInclude(p => p.User)
                .Include(c => c.CaseRequests)
                    .ThenInclude(cr => cr.SendCaseRequests)
                        .ThenInclude(sc => sc.Doctor).ThenInclude(d => d.User)
                .OrderByDescending(c => c.CreatedAt)
                .AsQueryable();

            if (take.HasValue) query = query.Take(take.Value);

            var casesData = await query.ToListAsync();

            return casesData.Select(c =>
            {
                var acceptedRequest = c.CaseRequests.FirstOrDefault(cr => cr.status == Status.Active);
                var studentName = acceptedRequest?.SendCaseRequests
                    .Select(sc => sc.Doctor?.User?.FullName)
                    .FirstOrDefault(n => n != null);

                return new AdminCaseRowDto
                {
                    CaseId = c.Id,
                    CaseType = c.Typies.ToString(),
                    PatientName = c.Patient?.User?.FullName ?? "Unknown",
                    StudentName = studentName,
                    Status = c.status,
                    CreatedAt = c.CreatedAt
                };
            }).ToList();
        }
    }
}