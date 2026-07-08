
using DentLink.BusinessLogicLayer.DTOs.Doctordtos;
using DentLink.DataAccessLayer.Models;

namespace DentLink.BusinessLogicLayer.Services.ServiceInterface
{
    public interface ISessionService
    {

        Task<ConfirmArrivalResultDto> ConfirmPatientArrivalAsync(int sessionId);
        Task<CompleteSessionResultDto> CompleteSessionAsync(int sessionId, string summaryMessage);
        Task<Session> GetSessionByIdAsync(int sessionId);
        Task<Session> CreateSessionAsync(Session session);
    }
}
