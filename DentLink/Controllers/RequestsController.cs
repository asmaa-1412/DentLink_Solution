using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DentLink.PresentionLayer.Controllers
{
    [Route("[controller]")]
    public class RequestsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequestsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        [HttpGet("case/{caseId}")]
        public async Task<IActionResult> GetRequestsByCaseId(int caseId)
        {
            var requests = await _unitOfWork.Repository<SelectCaseRequest>().FindAsync(r => r.CaseRequestId == caseId);

            return Json(requests);
        }



        [HttpPost("{requestId}/accept")]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            var request = await _unitOfWork.Repository<CaseRequest>().GetByIdAsync(requestId);
            if (request == null) return NotFound(new { Message = "Requset Not Found!" });

            if (request.status == DataAccessLayer.Enums.Status.Active)
            {
                return BadRequest(new { Message = "This Requset Accepted and Sessions Created Already." });
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

            return Json(new { Message = "Request Accepted and Session created successfully!" });
        }



        [HttpPost("{requestId}/reject")]
        public async Task<IActionResult> RejectRequest(int requestId)
        {
            var request = await _unitOfWork.Repository<CaseRequest>().GetByIdAsync(requestId);
            if (request == null) return NotFound(new { Message = "Requset Not Found!" });

            if (request.status == DataAccessLayer.Enums.Status.Active)
            {
                return BadRequest(new { Message = "Cannot reject requset Already Accepted And Sessions Is Active." });
            }

            request.status = DataAccessLayer.Enums.Status.Cancelled;
            await _unitOfWork.CompleteAsync();

            return Json(new { Message = "Request rejected successfully." });
        }
    }
}

