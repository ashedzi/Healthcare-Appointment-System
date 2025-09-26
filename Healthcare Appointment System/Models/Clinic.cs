namespace Healthcare_Appointment_System.Models {
    public class Clinic {
        public int ClinicId { get; set; }
        public string Name { get; set; }
        public string ClinicEmail { get; set; }
        public string ClinicAddress { get; set; }
        public TimeSpan StartOperatingHours { get; set; }
        public TimeSpan EndOperatingHours { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();
    }
}
