namespace Healthcare_Appointment_System.Models {
    public class ClinicDTO {
        public int ClinicId { get; set; }
        public string Name { get; set; }
        public string ClinicEmail { get; set; }
        public string ClinicAddress { get; set; }
        public string PhoneNumber { get; set; }

        public List<DoctorDTO> Doctors { get; set; } = new List<DoctorDTO>();
    }

    public class CreateClinicDTO {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string ClinicEmail { get; set; }
        public string ClinicAddress { get; set; }
        public TimeSpan StartOperatingHours { get; set; }
        public TimeSpan EndOperatingHours { get; set; }
    }

    public class UpdateClinicDTO : CreateClinicDTO {

    }

    public class ClinicScheduleDTO {
        public int ClinicId { get; set; }
        public string ClinicName { get; set; }
        public DateTime Date { get; set; }
        public string OperatingHours { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalAvailableSlots { get; set; }
        public int TotalDoctorsWorking { get; set; }
        public List<string> MorningDoctors { get; set; } = new List<string>();
        public List<string> EveningDoctors { get; set; } = new List<string>();
        public List<DoctorScheduleDTO> DoctorSchedules { get; set; } = new List<DoctorScheduleDTO>();
    }

    public class DoctorScheduleDTO {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialty { get; set; }
        public string Shift { get; set; }
        public string WorkingHours { get; set; }
        public int TotalAppointments { get; set; }
        public int AvailableSlots { get; set; }
        public List<ScheduledAppointmentDTO> Appointments { get; set; } = new List<ScheduledAppointmentDTO>();
    }

    public class ScheduledAppointmentDTO {
        public int AppointmentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Duration { get; set; }
        public string TimeSlot => $"{StartTime:hh:mm tt} - {EndTime:hh:mm tt}";
        public string PatientName { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }

}
