namespace Healthcare_Appointment_System.Models {
    public class DoctorSummaryDTO {
        public int DoctorId { get; set; }
        public string FullName { get; set; }   
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialty { get; set; }     
        public TimeSpan AvailableStart { get; set; }
        public TimeSpan AvailableEnd { get; set; }
        public int AppointmentDurationMinutes { get; set; }
    }

    public class CreateDoctorDTO {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialty { get; set; }
        public TimeSpan AvailableStart { get; set; }
        public TimeSpan AvailableEnd { get; set; }
        public int AppointmentDurationMinutes { get; set; }
    }

    public class UpdateDoctorDTO : CreateDoctorDTO {

    }
}
