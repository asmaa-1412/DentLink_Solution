using DentLink.DataAccessLayer.Models;
using DentLink.DataAccessLayer.Repositories;

namespace DentLink.DataAccessLayer.Contracts
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetPendingApprovalsAsync();
        Task<Doctor> GetDoctorWithCaseRequestsAsync(int doctorId);
    }

}