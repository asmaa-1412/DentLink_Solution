using DentLink.DataAccessLayer.Data;
using DentLink.DataAccessLayer.Models;
using DentLink.PresentionLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentLink.PresentionLayer.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult RoleSelection() => View();

        // ---------------- PATIENT ----------------

        [HttpGet]
        public IActionResult RegisterPatient() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPatient(PatientRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                FullName = model.Name,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.Phone
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, "Patient");

            var patient = new Patient
            {
                UserId = user.Id,
                Age = model.Age,
                Address = $"{model.Governorate} - {model.City}"
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Profile", "Patients", new { id = patient.Id });
        }

        // ---------------- STUDENT ----------------

        [HttpGet]
        public IActionResult RegisterStudent() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStudent(StudentRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                FullName = model.Name,
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, "Student");

            // حفظ صورة الكارنيه في wwwroot/uploads/idcards
            string idCardPath = null;
            if (model.StudentId != null && model.StudentId.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "idcards");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.StudentId.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.StudentId.CopyToAsync(stream);
                }

                idCardPath = $"/uploads/idcards/{fileName}";
            }

            var doctor = new Doctor
            {
                UserId = user.Id,
                University = model.University,
                Department = model.College,
                AcademicYear = model.AcademicYear,
                IdCardUrl = idCardPath,
                IsApproved = false,
                IsVerified = false
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            // مش هنعمل SignIn هنا، لأن الطالب لازم يستنى موافقة الـ Admin
            return RedirectToAction("PendingApproval");
        }

        [HttpGet]
        public IActionResult PendingApproval() => View("~/Views/student/student-pending.cshtml");
        // ---------------- LOGIN ----------------

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError("", "wrong password");
                return View(model);
            }

            if (await _userManager.IsInRoleAsync(user, "Student"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doctor == null || !doctor.IsApproved)
                {
                    ModelState.AddModelError("", "Your Account is under review");
                    return View(model);
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction("Dashboard", "Admin");

            if (await _userManager.IsInRoleAsync(user, "Patient"))
                return RedirectToAction("PatientDashboard", "Sessions");

            if (await _userManager.IsInRoleAsync(user, "Student"))
                return RedirectToAction("Dashboard", "Doctor");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}