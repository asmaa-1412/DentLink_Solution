using System;
using System.Collections.Generic;
using System.Text;
using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Data;
using DentLink.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DentLink.DataAccessLayer.Repositories
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        private readonly AppDbContext _context;

        public DoctorRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Doctor>> GetPendingApprovalsAsync()
        {
            return await _context.Doctors
                .Where(d => !d.IsApproved)
                .OrderBy(d => d.FullName)
                .ToListAsync();
        }
        public async Task<Doctor> GetDoctorWithCaseRequestsAsync(int doctorId)
        {
            return await _context.Doctors
                .Include(d => d.SendCaseRequests)
                    .ThenInclude(s => s.CaseRequest)
                        .ThenInclude(cr => cr.Case)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
        }



    }
}
