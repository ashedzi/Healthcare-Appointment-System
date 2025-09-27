using AutoMapper;
using Healthcare_Appointment_System.Models;

namespace Healthcare_Appointment_System.Mapping {
    public class AppointmentProfile : Profile {
        public AppointmentProfile() {
            //CreateMap<Models.Appointment, Models.AppointmentDTO>()
            //    .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName));
            //// So, you’re telling AutoMapper explicitly: “When mapping, fill AppointmentDTO.PatientName using Appointment.Patient.FullName.”
            //// so we don't have to manually set PatientName each time we map an Appointment to an AppointmentDTO.
            //CreateMap<Models.CreateAppointmentDTO, Models.Appointment>();

                CreateMap<Models.Appointment, Models.AppointmentDTO>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src =>
                    src.Patient != null ? src.Patient.FullName : "Unknown Patient"));
            CreateMap<Models.CreateAppointmentDTO, Models.Appointment>();
        }
    }
}
