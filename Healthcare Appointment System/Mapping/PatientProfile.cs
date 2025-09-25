using AutoMapper;

namespace Healthcare_Appointment_System.Mapping {
    public class PatientProfile : Profile {
        public PatientProfile() {
            CreateMap<Models.Patient, Models.PatientDTO>();
            CreateMap<Models.CreatePatientDTO, Models.Patient>();
            CreateMap<Models.UpdatePatientDTO, Models.Patient>();
        }
    }
}
