namespace Healthcare_Appointment_System.Models {
    public enum GenderList {
        Male,
        Female
    }
    public class Patient {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName {
            get { return $"{FirstName} {LastName}"; }
        }
        public int Age => DateTime.Now.Year - DateOfBirth.Year -
                 (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string EmergencyContact { get; set; }
        public string PhoneNumber { get; set; }
        public GenderList Gender { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
