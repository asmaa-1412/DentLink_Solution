using DentLink.BusinessLogicLayer.DTOs.PatientDTOs;
using DentLink.BusinessLogicLayer.Services.ServiceInterface;
using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DentLink.PresentationLayer.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IUnitOfWork _unitOfWork;

        public SessionsController(ISessionService sessionService, IUnitOfWork unitOfWork)
        {
            _sessionService = sessionService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> PatientDashboard(int id) 
        {
            var myCases = await _unitOfWork.Repository<Case>().FindAsync(c => c.PatientId == id);
            var myCasesList = myCases ?? Enumerable.Empty<Case>();
            var myCasesCount = myCasesList.Count();

            var pendingRequests = await _unitOfWork.Repository<SelectCaseRequest>().FindAsync(r => r.PatientId == id);
            var pendingRequestsCount = pendingRequests != null ? pendingRequests.Count() : 0;

            var completedSessions = await _unitOfWork.Repository<Session>().FindAsync(s => s.PatientArrived == true && s.SessionEnd != null);
            var completedSessionsCount = completedSessions != null ? completedSessions.Count() : 0;

            var recentCasesData = myCasesList
                .OrderByDescending(c => c.Id)
                .ToList(); 

            var dashboardData = new PatientDashboardDto
            {
                MyCasesCount = myCasesCount,
                PendingRequestsCount = pendingRequestsCount,
                CompletedSessionsCount = completedSessionsCount,
                RecentCases = recentCasesData   
            };

            return View(dashboardData);
        }
    }
}