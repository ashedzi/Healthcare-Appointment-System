namespace Healthcare_Appointment_System.Models {
    public class DoctorDTO {
        public int DoctorId { get; set; }
        public string FullName { get; set; }   
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Specialty DoctorSpecialty { get; set; }
        public string LicenseNumber { get; set; }  
        public TimeSpan AvailableStart { get; set; }
        public TimeSpan AvailableEnd { get; set; }
        public int AppointmentDurationMinutes { get; set; }
    }

    public class CreateDoctorDTO {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Specialty DoctorSpecialty { get; set; }  
        public string LicenseNumber { get; set; }  
        public TimeSpan AvailableStart { get; set; }
        public TimeSpan AvailableEnd { get; set; }
        public int AppointmentDurationMinutes { get; set; }
    }

    public class UpdateDoctorDTO : CreateDoctorDTO {

    }

    public class AvailableSlotDTO {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Duration { get; set; }
        public string TimeSlot => $"{StartTime:hh:mm tt} - {EndTime:hh:mm tt}";
        public int ClinicId { get; set; }
        public string ClinicName { get; set; }
    }

    public class AssignDoctorToClinicDTO {
        public int ClinicId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Shift DoctorShift { get; set; }
    }

    public class DoctorClinicAssignmentDTO {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int ClinicId { get; set; }
        public string ClinicName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Shift DoctorShift { get; set; }
        public string AssignmentPeriod => $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
    }
}
