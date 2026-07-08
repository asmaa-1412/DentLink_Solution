using DentLink.BusinessLogicLayer.DTOs.Doctordtos;
using DentLink.BusinessLogicLayer.Services.ServiceInterface;
using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Models; 

namespace DentLink.BusinessLogicLayer.Services.ServiceImplementation
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Session> GetSessionByIdAsync(int sessionId)
        {
            return await _unitOfWork.Repository<Session>().GetByIdAsync(sessionId);
        }

        public async Task<Session> CreateSessionAsync(Session session)
        {
            session.PatientArrived = false;

            await _unitOfWork.Repository<Session>().AddAsync(session);
            await _unitOfWork.CompleteAsync();
            return session;
        }

        public async Task<ConfirmArrivalResultDto> ConfirmPatientArrivalAsync(int sessionId)
        {
            var session = await _unitOfWork.Repository<Session>().GetByIdAsync(sessionId);

            if (session == null)
            {
                return new ConfirmArrivalResultDto { SessionId = sessionId, PatientArrived = false, Message = "Session not found." };
            }

            session.PatientArrived = true;
            session.SessionStart = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync();

            return new ConfirmArrivalResultDto
            {
                SessionId = session.Id,
                PatientArrived = session.PatientArrived,
                Message = session.PatientArrived
                     ? "Patient arrival confirmed and session started."
                     : "Failed to confirm patient arrival."
            };
        }
        public async Task<CompleteSessionResultDto> CompleteSessionAsync(int sessionId, string summaryMessage)
        {
            var session = await _unitOfWork.Repository<Session>().GetByIdAsync(sessionId);

            if (session == null)
            {
                return new CompleteSessionResultDto { SessionId = sessionId, SessionEnd = DateTime.MinValue, Message = "Session not found." };
            }

            session.SessionEnd = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync();

            return new CompleteSessionResultDto
            {
                SessionId = session.Id,
                SessionEnd = session.SessionEnd.Value, 
                Message = "Session completed successfully."
            };
        }
    }
}