namespace Healthcare_Appointment_System.Models {
    public enum Specialty {
        GeneralPractitioner,  
        Pediatrician,   
        Cardiologist,     
        Dermatologist,    
        Neurologist,        
        Orthopedic,          
        Gynecologist,       
        Psychiatrist,     
        Dentist,               
        Ophthalmologist
    }
    public class Doctor {
        public int DoctorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName {
            get { return $"{FirstName} {LastName}"; }
        }
        public string Email { get; set; }
        public string LicenseNumber { get; set; }
        public Specialty DoctorSpecialty { get; set; }
        public string PhoneNumber { get; set; }
        public TimeSpan AvailableStart { get; set; } = new TimeSpan(9, 0, 0);
        public TimeSpan AvailableEnd { get; set; } = new TimeSpan(17, 0, 0);  
        public int AppointmentDurationMinutes { get; set; } = 30;
        public ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
