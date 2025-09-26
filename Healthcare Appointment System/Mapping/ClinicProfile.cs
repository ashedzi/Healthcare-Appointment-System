using AutoMapper;
using Healthcare_Appointment_System.Models;

namespace Healthcare_Appointment_System.Mapping {
    public class ClinicProfile : Profile {
        public ClinicProfile() {
            CreateMap<Clinic, ClinicDTO>()
                .ForMember(dest => dest.Doctors,
                    opt => opt.MapFrom(src => src.DoctorClinics.Select(dc => dc.Doctor)));

            CreateMap<CreateClinicDTO, Clinic>();

            CreateMap<UpdateClinicDTO, Clinic>();
        }
    }
}
