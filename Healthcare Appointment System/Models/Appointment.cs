namespace Healthcare_Appointment_System.Models {
    public class Appointment {
        public int AppointmentId { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public DateTime EndTime => StartTime.AddMinutes(Duration);
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public string Note { get; set; }
        public enum AppointementReasons {
            GeneralCheckup,       
            FollowUp,             
            NewSymptoms,          
            ChronicCondition,     
            PrescriptionRenewal,  
            LabResults,           
            Vaccination,          
            SpecialistReferral,   
            Emergency,            
            Consultation
        }
        public AppointementReasons Reason { get; set; }

        public enum Status {
            Scheduled,
            Completed,
            Cancelled,
            NoShow
        }
        public Status AppointmentStatus { get; set; }
    }
}
