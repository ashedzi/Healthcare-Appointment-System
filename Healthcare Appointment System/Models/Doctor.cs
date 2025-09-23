namespace Healthcare_Appointment_System.Models {
    public class Doctor {
        public int DoctorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName {
            get { return $"{FirstName}{LastName}"; }
        }
        public string Email { get; set; }
        public int LicenseNumber { get; set; }
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

        //public enum Availability {

        //}
        public Specialty DoctorSpecialty { get; set; }
        public string PhoneNumber { get; set; }
        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
