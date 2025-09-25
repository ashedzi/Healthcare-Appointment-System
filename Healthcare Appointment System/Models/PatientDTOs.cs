namespace Healthcare_Appointment_System.Models {
    public class PatientDTO {
        public int PatientId { get; set; }
        public string FullName { get; set; }
        public string PatientEmail { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public GenderList Gender { get; set; }
    }

    public class CreatePatientDTO {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PatientEmail { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public GenderList Gender { get; set; }
        public string EmergencyContact { get; set; }
    }

    public class UpdatePatientDTO : CreatePatientDTO { }
}
