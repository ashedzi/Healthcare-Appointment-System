using Microsoft.EntityFrameworkCore;

namespace Healthcare_Appointment_System.Models {
    public class HealthcareAppointmentSystemContext: DbContext {
        public DbSet<Doctor> Doctors { get; set;}
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public HealthcareAppointmentSystemContext(DbContextOptions<HealthcareAppointmentSystemContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Doctor>()
                .HasKey(d => d.ID)
        }

    }
}
