using AutoMapper;
using Healthcare_Appointment_System.Models;

namespace Healthcare_Appointment_System.Mapping {
    public class DoctorProfile : Profile {
        public DoctorProfile() {
            CreateMap<Doctor, DoctorDTO>();
            CreateMap<Appointment, AppointmentDTO>();
            CreateMap<CreateDoctorDTO, Doctor>();
            CreateMap<UpdateDoctorDTO, Doctor>();
        }
    }
}
