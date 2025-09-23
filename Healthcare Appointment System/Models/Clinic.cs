namespace Healthcare_Appointment_System.Models {
    public class Clinic {
        public int ClinicId { get; set; }
        public string Name { get; set; }
        public string email { get; set; }
        public string Address { get; set; }
        public TimeSpan StartAvailableHours { get; set; }
        public TimeSpan EndAvailableHours { get; set; }
        public string ContactNumber { get; set; }
        public ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();
    }
}
