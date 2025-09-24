using Microsoft.EntityFrameworkCore;

namespace Healthcare_Appointment_System.Models {
    public class HealthcareAppointmentSystemContext: DbContext {
        public DbSet<Doctor> Doctors { get; set;}
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorClinic> DoctorClinics { get; set; }

        public HealthcareAppointmentSystemContext(DbContextOptions<HealthcareAppointmentSystemContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // ===== Doctor =====
            modelBuilder.Entity<Doctor>()
                .HasKey(d => d.DoctorId);

            modelBuilder.Entity<Doctor>()
                .Property(d => d.FirstName)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Doctor>()
                .Property(d => d.LastName)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Doctor>()
                .Property(d => d.Email)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Doctor>()
                .Property(d => d.LicenseNumber)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<Doctor>()
                .Property(d => d.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);

            // ===== Patient =====
            modelBuilder.Entity<Patient>()
                .HasKey(p => p.PatientId);

            modelBuilder.Entity<Patient>()
                .Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Patient>()
                .Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Patient>()
                .Property(p => p.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);

            // ===== Clinic =====
            modelBuilder.Entity<Clinic>()
                .HasKey(c => c.ClinicId);

            modelBuilder.Entity<Clinic>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Clinic>()
                .Property(c => c.ClinicEmail)
                .HasMaxLength(50);

            modelBuilder.Entity<Clinic>()
                .Property(c => c.ContactNumber)
                .HasMaxLength(15);

            // ===== Appointment =====
            modelBuilder.Entity<Appointment>()
                .HasKey(a => a.AppointmentId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Clinic)
                .WithMany()
                .HasForeignKey(a => a.ClinicId);

            // ===== DoctorClinic (many-to-many with extra fields) =====
            modelBuilder.Entity<DoctorClinic>()
                .HasKey(dc => new { dc.DoctorId, dc.ClinicId });

            modelBuilder.Entity<DoctorClinic>()
                .HasOne(dc => dc.Doctor)
                .WithMany(d => d.DoctorClinics)
                .HasForeignKey(dc => dc.DoctorId);

            modelBuilder.Entity<DoctorClinic>()
                .HasOne(dc => dc.Clinic)
                .WithMany(c => c.DoctorClinics)
                .HasForeignKey(dc => dc.ClinicId);
        }

    }
}
