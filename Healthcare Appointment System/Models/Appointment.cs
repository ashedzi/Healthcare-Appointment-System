namespace Healthcare_Appointment_System.Models {
    public enum AppointmentReasons {
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
    public enum Status {
        Scheduled,
        Completed,
        Cancelled,
        NoShow
    }
    public class Appointment {
        public int AppointmentId { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public DateTime EndTime => StartTime.AddMinutes(Duration);
        public string Slot => $"{StartTime:hh:mm tt} - {EndTime:hh:mm tt}";
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; }
        public string Note { get; set; }
        public AppointmentReasons Reason { get; set; }
        public Status AppointmentStatus { get; set; }
    }
}
