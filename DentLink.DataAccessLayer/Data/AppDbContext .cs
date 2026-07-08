
using DentLink.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DentLink.DataAccessLayer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseRequest> CaseRequests { get; set; }
        public DbSet<SendCaseRequest> SendCaseRequests { get; set; }
        public DbSet<SelectCaseRequest> SelectCaseRequests { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SendCaseRequest>()
                .HasKey(s => new { s.DoctorId, s.CaseRequestId });

            modelBuilder.Entity<SendCaseRequest>()
                .HasOne(s => s.Doctor)
                .WithMany(d => d.SendCaseRequests)
                .HasForeignKey(s => s.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SendCaseRequest>()
                .HasOne(s => s.CaseRequest)
                .WithMany(cr => cr.SendCaseRequests)
                .HasForeignKey(s => s.CaseRequestId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<SelectCaseRequest>()
                .HasKey(s => new { s.CaseRequestId, s.PatientId });

            modelBuilder.Entity<SelectCaseRequest>()
                .HasOne(s => s.CaseRequest)
                .WithMany(cr => cr.SelectCaseRequests)
                .HasForeignKey(s => s.CaseRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SelectCaseRequest>()
                .HasOne(s => s.Patient)
                .WithMany(p => p.SelectCaseRequests)
                .HasForeignKey(s => s.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Case>()
                .HasOne(c => c.Patient)
                .WithMany(p => p.Cases)
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<CaseRequest>()
                .HasOne(cr => cr.Case)
                .WithMany(c => c.CaseRequests)
                .HasForeignKey(cr => cr.CaseId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Session>()
                .HasOne(s => s.CaseRequest)
                .WithOne(cr => cr.Session)
                .HasForeignKey<Session>(s => s.CaseRequestId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<CaseRequest>()
                .Property(cr => cr.TransportCost)
                .HasColumnType("decimal(10,2)");
        }
    }
}
