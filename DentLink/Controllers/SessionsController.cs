using DentLink.BusinessLogicLayer.DTOs.PatientDTOs;
using DentLink.BusinessLogicLayer.Services.ServiceInterface;
using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DentLink.PresentationLayer.Controllers
{
    [Route("[controller]")]
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IUnitOfWork _unitOfWork;

        public SessionsController(ISessionService sessionService, IUnitOfWork unitOfWork)
        {
            _sessionService = sessionService;
            _unitOfWork = unitOfWork;
        }






        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPatientDashboard(int patientId)
        {
            var myCases = await _unitOfWork.Repository<Case>().FindAsync(c => c.PatientId == patientId);
            var myCasesList = myCases ?? Enumerable.Empty<Case>();
            var myCasesCount = myCasesList.Count();

            var pendingRequests = await _unitOfWork.Repository<SelectCaseRequest>().FindAsync(r => r.PatientId == patientId);
            var pendingRequestsCount = pendingRequests != null ? pendingRequests.Count() : 0;

            var completedSessions = await _unitOfWork.Repository<Session>().FindAsync(s => s.PatientArrived == true && s.SessionEnd != null);
            var completedSessionsCount = completedSessions != null ? completedSessions.Count() : 0;

            var recentCasesData = myCasesList
                .OrderByDescending(c => c.Id)                      
                .Select(c => new
                {
                    c.Id,
                    Title = c.Typies.ToString(),
                    c.Description,
                    Status = "pending", 
                    RequestsCount = pendingRequestsCount 
                });

            var dashboardData = new PatientDashboardDto
            {
                MyCasesCount = myCasesCount,
                PendingRequestsCount = pendingRequestsCount,
                CompletedSessionsCount = completedSessionsCount,
                RecentCases = recentCasesData
            };

            return Json(dashboardData);
        }
    }
}