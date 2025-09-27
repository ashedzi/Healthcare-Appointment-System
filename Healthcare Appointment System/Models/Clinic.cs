using System.ComponentModel.DataAnnotations;

namespace Healthcare_Appointment_System.Models {
    public class Clinic {
        public int ClinicId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string ClinicEmail { get; set; }

        [Required]
        [MaxLength(200)]
        public string ClinicAddress { get; set; }

        [Required]
        public TimeSpan StartOperatingHours { get; set; }

        [Required]
        public TimeSpan EndOperatingHours { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
        public ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();
    }
}
