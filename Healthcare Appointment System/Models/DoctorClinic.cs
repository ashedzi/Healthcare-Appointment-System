namespace Healthcare_Appointment_System.Models {
    public enum Shift {
        Morning,
        Evening
    }
    public class DoctorClinic {
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Shift DoctorShift { get; set; }
    }
}
