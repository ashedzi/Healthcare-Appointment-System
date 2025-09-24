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

            //seed data 
            // Clinics
            modelBuilder.Entity<Clinic>().HasData(
                new Clinic { ClinicId = 1, Name = "City Health Clinic", ClinicEmail = "info@cityhealth.com", ClinicAddress = "123 Main St", StartOperatingHours = new TimeSpan(8, 0, 0), EndOperatingHours = new TimeSpan(17, 0, 0), ContactNumber = "555-1000" },
                new Clinic { ClinicId = 2, Name = "Downtown Medical Center", ClinicEmail = "contact@downtownmed.com", ClinicAddress = "456 Elm Ave", StartOperatingHours = new TimeSpan(9, 0, 0), EndOperatingHours = new TimeSpan(18, 0, 0), ContactNumber = "555-2000" },
                new Clinic { ClinicId = 3, Name = "Suburban Family Clinic", ClinicEmail = "hello@suburbanclinic.com", ClinicAddress = "789 Oak Blvd", StartOperatingHours = new TimeSpan(7, 0, 0), EndOperatingHours = new TimeSpan(16, 0, 0), ContactNumber = "555-3000" },
                new Clinic { ClinicId = 4, Name = "Specialist Care Center", ClinicEmail = "info@specialistcare.com", ClinicAddress = "101 Pine Rd", StartOperatingHours = new TimeSpan(10, 0, 0), EndOperatingHours = new TimeSpan(19, 0, 0), ContactNumber = "555-4000" },
                new Clinic { ClinicId = 5, Name = "Health & Wellness Clinic", ClinicEmail = "contact@wellnessclinic.com", ClinicAddress = "202 Maple St", StartOperatingHours = new TimeSpan(8, 30, 0), EndOperatingHours = new TimeSpan(17, 30, 0), ContactNumber = "555-5000" }
            );

            // Doctors
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { DoctorId = 1, FirstName = "John", LastName = "Doe", Email = "johndoe@example.com", PhoneNumber = "555-1111", LicenseNumber = "LIC001", DoctorSpecialty = Specialty.Cardiologist, AvailableStart = new TimeSpan(9, 0, 0), AvailableEnd = new TimeSpan(17, 0, 0), AppointmentDurationMinutes = 30 },
                new Doctor { DoctorId = 2, FirstName = "Jane", LastName = "Smith", Email = "janesmith@example.com", PhoneNumber = "555-2222", LicenseNumber = "LIC002", DoctorSpecialty = Specialty.Dermatologist, AvailableStart = new TimeSpan(10, 0, 0), AvailableEnd = new TimeSpan(18, 0, 0), AppointmentDurationMinutes = 30 },
                new Doctor { DoctorId = 3, FirstName = "Alice", LastName = "Brown", Email = "alicebrown@example.com", PhoneNumber = "555-3333", LicenseNumber = "LIC003", DoctorSpecialty = Specialty.Pediatrician, AvailableStart = new TimeSpan(8, 0, 0), AvailableEnd = new TimeSpan(16, 0, 0), AppointmentDurationMinutes = 30 },
                new Doctor { DoctorId = 4, FirstName = "Michael", LastName = "Green", Email = "michaelgreen@example.com", PhoneNumber = "555-4444", LicenseNumber = "LIC004", DoctorSpecialty = Specialty.Neurologist, AvailableStart = new TimeSpan(9, 0, 0), AvailableEnd = new TimeSpan(17, 0, 0), AppointmentDurationMinutes = 45 },
                new Doctor { DoctorId = 5, FirstName = "Laura", LastName = "White", Email = "laurawhite@example.com", PhoneNumber = "555-5555", LicenseNumber = "LIC005", DoctorSpecialty = Specialty.Gynecologist, AvailableStart = new TimeSpan(7, 30, 0), AvailableEnd = new TimeSpan(15, 30, 0), AppointmentDurationMinutes = 30 }
            );

            // Patients
            modelBuilder.Entity<Patient>().HasData(
                new Patient { PatientId = 1, FirstName = "Tom", LastName = "Anderson", DateOfBirth = new DateTime(1990, 5, 12), Address = "12 River Rd", EmergencyContact = "555-9000", PhoneNumber = "555-6001", Gender = GenderList.Male },
                new Patient { PatientId = 2, FirstName = "Emily", LastName = "Clark", DateOfBirth = new DateTime(1985, 11, 23), Address = "34 Lake St", EmergencyContact = "555-9001", PhoneNumber = "555-6002", Gender = GenderList.Female },
                new Patient { PatientId = 3, FirstName = "David", LastName = "Lee", DateOfBirth = new DateTime(2000, 3, 5), Address = "56 Hill Ave", EmergencyContact = "555-9002", PhoneNumber = "555-6003", Gender = GenderList.Male },
                new Patient { PatientId = 4, FirstName = "Sophia", LastName = "Taylor", DateOfBirth = new DateTime(1995, 7, 17), Address = "78 Pine St", EmergencyContact = "555-9003", PhoneNumber = "555-6004", Gender = GenderList.Female },
                new Patient { PatientId = 5, FirstName = "James", LastName = "Wilson", DateOfBirth = new DateTime(1988, 1, 30), Address = "90 Oak Rd", EmergencyContact = "555-9004", PhoneNumber = "555-6005", Gender = GenderList.Male }
            );

            // DoctorClinic relationships
            modelBuilder.Entity<DoctorClinic>().HasData(
                new DoctorClinic { DoctorId = 1, ClinicId = 1, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), DoctorShift = Shift.Morning },
                new DoctorClinic { DoctorId = 2, ClinicId = 2, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), DoctorShift = Shift.Evening },
                new DoctorClinic { DoctorId = 3, ClinicId = 3, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), DoctorShift = Shift.Morning },
                new DoctorClinic { DoctorId = 4, ClinicId = 4, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), DoctorShift = Shift.Evening },
                new DoctorClinic { DoctorId = 5, ClinicId = 5, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), DoctorShift = Shift.Morning }
            );

            // Appointments
            modelBuilder.Entity<Appointment>().HasData(
                new Appointment { AppointmentId = 1, DoctorId = 1, PatientId = 1, ClinicId = 1, StartTime = new DateTime(2025, 9, 25, 9, 0, 0), Duration = 30, Note = "Routine checkup", Reason = AppointmentReasons.GeneralCheckup, AppointmentStatus = Status.Scheduled },
                new Appointment { AppointmentId = 2, DoctorId = 2, PatientId = 2, ClinicId = 2, StartTime = new DateTime(2025, 9, 25, 15, 0, 0), Duration = 45, Note = "Skin rash consultation", Reason = AppointmentReasons.NewSymptoms, AppointmentStatus = Status.Scheduled },
                new Appointment { AppointmentId = 3, DoctorId = 3, PatientId = 3, ClinicId = 3, StartTime = new DateTime(2025, 9, 26, 10, 0, 0), Duration = 30, Note = "Pediatric follow-up", Reason = AppointmentReasons.FollowUp, AppointmentStatus = Status.Scheduled },
                new Appointment { AppointmentId = 4, DoctorId = 4, PatientId = 4, ClinicId = 4, StartTime = new DateTime(2025, 9, 26, 14, 0, 0), Duration = 45, Note = "Neurology consultation", Reason = AppointmentReasons.Consultation, AppointmentStatus = Status.Scheduled },
                new Appointment { AppointmentId = 5, DoctorId = 5, PatientId = 5, ClinicId = 5, StartTime = new DateTime(2025, 9, 27, 8, 30, 0), Duration = 30, Note = "Gynecology checkup", Reason = AppointmentReasons.GeneralCheckup, AppointmentStatus = Status.Scheduled },
                new Appointment { AppointmentId = 6, DoctorId = 1, PatientId = 2, ClinicId = 1, StartTime = new DateTime(2025, 9, 27, 10, 0, 0), Duration = 30, Note = "Cardio follow-up", Reason = AppointmentReasons.FollowUp, AppointmentStatus = Status.Scheduled }
            );

        }

    }
}
