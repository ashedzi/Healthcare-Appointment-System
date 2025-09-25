namespace Healthcare_Appointment_System.Models {
    public class AppointmentDTO {
        public int AppointmentId { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string Slot { get; set; }
        public string Note { get; set; }
        public AppointmentReasons Reason { get; set; }
        public Status AppointmentStatus { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
    }

    public class CreateAppointmentDTO {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public AppointmentReasons Reason { get; set; }
        public string Note { get; set; }
    }
}
