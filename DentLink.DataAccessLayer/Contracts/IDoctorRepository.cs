using DentLink.DataAccessLayer.Models;

namespace DentLink.DataAccessLayer.Contracts
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetPendingApprovalsAsync();
        Task<Doctor> GetDoctorWithCaseRequestsAsync(int doctorId);
    }
}
