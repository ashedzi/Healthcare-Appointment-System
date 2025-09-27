using System.ComponentModel.DataAnnotations;

namespace Healthcare_Appointment_System.Models {
    public enum Specialty {
        GeneralPractitioner,  
        Pediatrician,   
        Cardiologist,     
        Dermatologist,    
        Neurologist,        
        Orthopedic,          
        Gynecologist,       
        Psychiatrist,     
        Dentist,               
        Ophthalmologist
    }
    public class Doctor {
        public int DoctorId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        public string FullName {
            get { return $"{FirstName} {LastName}"; }
        }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string LicenseNumber { get; set; }

        [Required]
        public Specialty DoctorSpecialty { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }

        [Required]
        public TimeSpan AvailableStart { get; set; } = new TimeSpan(9, 0, 0);

        [Required]
        public TimeSpan AvailableEnd { get; set; } = new TimeSpan(17, 0, 0);

        [Required]
        [Range(5, 480)]
        public int AppointmentDurationMinutes { get; set; } = 30;
        public ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
