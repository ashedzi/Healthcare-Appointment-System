using System.ComponentModel.DataAnnotations;

namespace Healthcare_Appointment_System.Models {
    public enum AppointmentReasons {
        GeneralCheckup,       
        FollowUp,             
        NewSymptoms,          
        ChronicCondition,     
        PrescriptionRenewal,  
        LabResults,           
        Vaccination,          
        SpecialistReferral,   
        Emergency,            
        Consultation
    }
    public enum Status {
        Scheduled,
        Completed,
        Cancelled,
        NoShow
    }
    public class Appointment {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }

        [Required]
        [Range(5, 480, ErrorMessage = "Duration must be between 5 and 480 minutes.")]
        public int Duration { get; set; }
        public DateTime EndTime => StartTime.AddMinutes(Duration);
        public string Slot => $"{StartTime:hh:mm tt} - {EndTime:hh:mm tt}";

        [Required]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; }

        [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
        public string Note { get; set; }

        [Required]
        public AppointmentReasons Reason { get; set; }

        [Required]
        public Status AppointmentStatus { get; set; }
    }
}
