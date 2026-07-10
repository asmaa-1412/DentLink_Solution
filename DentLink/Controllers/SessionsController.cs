using DentLink.BusinessLogicLayer.DTOs.PatientDTOs;
using DentLink.BusinessLogicLayer.Services.ServiceInterface;
using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DentLink.PresentationLayer.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public SessionsController(ISessionService sessionService, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _sessionService = sessionService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<IActionResult> PatientDashboard() 
        {
            var userId = _userManager.GetUserId(User);

            var patient = await _unitOfWork.Repository<Patient>()
                .GetEntityWithSpec(p => p.UserId == userId, p => p.User);

            if (patient == null) return NotFound();

            int id = patient.Id;

            var myCases = await _unitOfWork.Repository<Case>().FindAsync(c => c.PatientId == id);
            var myCasesList = (myCases ?? Enumerable.Empty<Case>()).ToList();

            var activeCasesCount = myCasesList.Count(c => c.status == DataAccessLayer.Enums.Status.Active);
            var completedCasesCount = myCasesList.Count(c => c.status == DataAccessLayer.Enums.Status.Completed);

            var pendingRequests = await _unitOfWork.Repository<SelectCaseRequest>().FindAsync(r => r.PatientId == id);
            var pendingRequestsCount = pendingRequests?.Count() ?? 0;

            
            var completedSessions = await _unitOfWork.Repository<Session>().FindAsync(
                s => s.PatientArrived == true
                     && s.SessionEnd != null
                     && s.CaseRequest.Case.PatientId == id);

            var completedSessionsList = (completedSessions ?? Enumerable.Empty<Session>()).ToList();

            var dashboardData = new PatientDashboardDto
            {
                PatientId = patient.Id,
                PatientName = patient.User?.FullName,
                MyCasesCount = myCasesList.Count,
                ActiveCasesCount = activeCasesCount,
                CompletedCasesCount = completedCasesCount,
                PendingRequestsCount = pendingRequestsCount,
                CompletedSessionsCount = completedSessionsList.Count,
                LastCompletedSessionDate = completedSessionsList
                    .OrderByDescending(s => s.SessionEnd)
                    .FirstOrDefault()?.SessionEnd,
                RecentCases = myCasesList.OrderByDescending(c => c.Id).ToList()
            };

            return View("~/Views/Patient/patient-dashboard.cshtml", dashboardData);
        }
    }
}