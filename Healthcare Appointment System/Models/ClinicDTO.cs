namespace Healthcare_Appointment_System.Models {
    public class ClinicDTO {
        public int ClinicId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class CreateClinicDTO {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UpdateClinicDTO : CreateClinicDTO {

    }
}
