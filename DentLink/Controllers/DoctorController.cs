using DentLink.DataAccessLayer.Data;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentLink.PresentionLayer.Controllers
{
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. Dashboard Action (مفتوحة دائماً للمشاهدة)
        // ==========================================
        public async Task<IActionResult> Dashboard()
        {
            // جلب بيانات الدكتور الحالي (نعتبره ID 1 للتجربة)
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == 1);

            // --- تم حذف شرط الـ Redirect بناءً على طلبك ---
            // الآن الصفحة ستفتح مباشرة حتى لو IsApproved = false ليتمكن من المشاهدة

            ViewBag.DoctorName = doctor?.FullName ?? "Omar Khaled";
            // نمرر حالة الحساب للـ View ربما نحتاجها لعرض تنبيه بسيط فوق
            ViewBag.IsApproved = doctor?.IsApproved ?? false;

            // حساب العدادات الحية
            ViewBag.AvailableCasesCount = await _context.Cases.CountAsync(c => c.status == "Available");
            ViewBag.AcceptedRequestsCount = await _context.CaseRequests.CountAsync(cr => cr.status == "Accepted");
            ViewBag.CompletedSessionsCount = await _context.Sessions.CountAsync(s => s.status == "completed");

            // جلب الأنشطة الأخيرة (الطلبات المقدمة)
            var recentRequests = await _context.SendCaseRequests
                .Include(s => s.CaseRequest)
                    .ThenInclude(cr => cr.Case)
                .OrderByDescending(s => s.CaseRequest.CreatedAt)
                .Take(3)
                .ToListAsync();
            ViewBag.RecentRequests = recentRequests;

            // جلب الجلسات القادمة غير المنتهية
            var upcomingSessions = await _context.Sessions
                .Include(s => s.CaseRequest)
                    .ThenInclude(cr => cr.Case)
                        .ThenInclude(c => c.Patient)
                .Where(s => s.status != "completed")
                .OrderBy(s => s.SessionDate)
                .Take(3)
                .ToListAsync();

            return View(upcomingSessions);
        }

        // ==========================================
        // 2. Under Review Action
        // ==========================================
        public IActionResult UnderReview()
        {
            // صفحة ثابتة تعرض رسالة الانتظار
            return View();
        }

        // ==========================================
        // 3. Available Cases Action (مفتوحة للمشاهدة)
        // ==========================================
        public async Task<IActionResult> AvailableCases(string searchQuery, string selectedType)
        {
            // 1. جلب الحالات المتاحة كـ Queryable لتجهيز الفلاتر
            var casesQuery = _context.Cases
                .Include(c => c.Patient)
                .Where(c => c.status == "Available")
                .AsQueryable();

            // 2. فلترة بالبحث (مع تحويل الكلمات لـ Lowercase لتجنب حساسية الحروف الكبيرة والصغيرة)
            if (!string.IsNullOrEmpty(searchQuery))
            {
                string searchLower = searchQuery.ToLower().Trim();

                casesQuery = casesQuery.Where(c =>
                    c.Type.ToLower().Contains(searchLower) ||
                    c.Description.ToLower().Contains(searchLower)
                );
            }

            // 3. فلترة بالتصنيف (Category Buttons)
            if (!string.IsNullOrEmpty(selectedType) && selectedType != "All")
            {
                casesQuery = casesQuery.Where(c => c.Type == selectedType);
            }

            // 4. تنفيذ الاستعلام وترتيب الأحدث أولاً
            var availableCases = await casesQuery
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            // 5. تمرير القيم للـ View عشان خانة البحث والـ Active Button يفضلوا محتفظين بحالتهم
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

            // 1. التشخيص: هل الدكتور مؤكد من الأدمن؟
            var doctor = await _context.Doctors.FindAsync(currentDoctorId);
            if (doctor == null || !doctor.IsApproved)
            {
                // إذا لم يكن مؤكداً، نمنعه ونرجعه لصفحة المراجعة
                return RedirectToAction(nameof(UnderReview));
            }

            // 2. كود التنفيذ الطبيعي (لا ينفذ إلا إذا كان مؤكداً)
            bool alreadyRequested = await _context.SendCaseRequests
                .AnyAsync(s => s.DoctorId == currentDoctorId && s.CaseRequest.CaseId == caseId);

            if (alreadyRequested)
            {
                return RedirectToAction(nameof(AvailableCases));
            }

            var newRequest = new CaseRequest
            {
                CaseId = caseId,
                status = "pending",
                TransportCost = transportCost,
                CreatedAt = DateTime.UtcNow
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
        // 4. Sessions Action (مفتوحة للمشاهدة)
        // ==========================================
        public async Task<IActionResult> Sessions()
        {
            int currentDoctorId = 1;

            var doctorSessions = await _context.Sessions
                .Include(s => s.CaseRequest)
                    .ThenInclude(cr => cr.Case)
                        .ThenInclude(c => c.Patient)
                .Where(session => session.CaseRequest.SendCaseRequests.Any(s => s.DoctorId == currentDoctorId))
                .OrderByDescending(session => session.st)
                .ToListAsync();

            return View(doctorSessions);
        }

        // --- أكشن حساس: يحتاج موافقة الأدمن ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmArrival(int sessionId)
        {
            int currentDoctorId = 1;

            // 1. التشخيص: هل الدكتور مؤكد؟
            var doctor = await _context.Doctors.FindAsync(currentDoctorId);
            if (doctor == null || !doctor.IsApproved)
            {
                return RedirectToAction(nameof(UnderReview));
            }

            // 2. كود التنفيذ الطبيعي
            var session = await _context.Sessions.FindAsync(sessionId);
            if (session != null)
            {
                session.IsPatientArrived = true;
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

            // 1. التشخيص: هل الدكتور مؤكد؟
            var doctor = await _context.Doctors.FindAsync(currentDoctorId);
            if (doctor == null || !doctor.IsApproved)
            {
                return RedirectToAction(nameof(UnderReview));
            }

            // 2. كود التنفيذ الطبيعي
            var session = await _context.Sessions.FindAsync(sessionId);
            if (session != null)
            {
                session.IsCompleted = true;
                session.Status = "completed";
                session.ActualEndTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Sessions));
        }

        // ==========================================
        // 5. Profile Actions (مفتوحة للتعديل في أي وقت)
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == 1);
            if (doctor == null)
            {
                doctor = new Doctor { FullName = "Omar Khaled", Email = "omar.k@cu.edu.eg" };
            }
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(Doctor updatedDoctor, IFormFile? ProfilePictureFile)
        {
            var doctorInDb = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == updatedDoctor.Id);
            if (doctorInDb == null) return NotFound();

            // معالجة رفع الصورة
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

            // تحديث باقي البيانات
            doctorInDb.FullName = updatedDoctor.FullName;
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