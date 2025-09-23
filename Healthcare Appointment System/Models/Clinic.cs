namespace Healthcare_Appointment_System.Models {
    public class Clinic {
        public int ClinicId { get; set; }
        public string Name { get; set; }
        public string email { get; set; }
        public string Address { get; set; }
        public string AvailableHours { get; set; }
        public string ContactNumber { get; set; }
        //public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        //public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    }
}
