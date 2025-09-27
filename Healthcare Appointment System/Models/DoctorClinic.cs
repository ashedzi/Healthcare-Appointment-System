using System.ComponentModel.DataAnnotations;

namespace Healthcare_Appointment_System.Models {
    public enum Shift {
        Morning,
        Evening
    }
    public class DoctorClinic {
        [Required]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        [Required]
        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public Shift DoctorShift { get; set; }
    }
}
