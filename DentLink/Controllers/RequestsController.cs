using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DentLink.PresentionLayer.Controllers
{
    public class RequestsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequestsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetRequestsByCaseId(int id) 
        {
            var requests = await _unitOfWork.Repository<SelectCaseRequest>().FindAsync(r => r.CaseRequestId == id);

            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            var request = await _unitOfWork.Repository<CaseRequest>().GetByIdAsync(requestId);
            if (request == null) return NotFound();

            if (request.status == DataAccessLayer.Enums.Status.Active)
            {
                TempData["Error"] = "This Request is already accepted!";
                return RedirectToAction("GetRequestsByCaseId", new { id = request.CaseId });
            }

            request.status = DataAccessLayer.Enums.Status.Active;

            var currentCase = await _unitOfWork.Repository<Case>().GetByIdAsync(request.CaseId);
            if (currentCase != null)
            {
                currentCase.status = DataAccessLayer.Enums.Status.Active;
            }

            var newSession = new Session
            {
                CaseRequestId = request.Id,
                SessionStart = DateTime.UtcNow,
                PatientArrived = false
            };

            await _unitOfWork.Repository<Session>().AddAsync(newSession);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("GetRequestsByCaseId", new { id = request.CaseId });
        }

        [HttpPost]
        public async Task<IActionResult> RejectRequest(int requestId)
        {
            var request = await _unitOfWork.Repository<CaseRequest>().GetByIdAsync(requestId);
            if (request == null) return NotFound();

            if (request.status == DataAccessLayer.Enums.Status.Active)
            {
                return BadRequest();
            }

            request.status = DataAccessLayer.Enums.Status.Cancelled;
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("GetRequestsByCaseId", new { id = request.CaseId });
        }
    }
}