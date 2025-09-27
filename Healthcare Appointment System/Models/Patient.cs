using System.ComponentModel.DataAnnotations;

namespace Healthcare_Appointment_System.Models {
    public enum GenderList {
        Male,
        Female
    }
    public class Patient {
        public int PatientId { get; set; }
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
        public string PatientEmail { get; set; }
        public int Age => DateTime.Now.Year - DateOfBirth.Year -
                 (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string EmergencyContact { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        public bool IsDeleted { get; set; } = false;

        [Required]
        public GenderList Gender { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
