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
        public string ClinicAddress { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UpdateClinicDTO : CreateClinicDTO {

    }
}
