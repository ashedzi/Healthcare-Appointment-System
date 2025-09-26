using AutoMapper;
using Healthcare_Appointment_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Healthcare_Appointment_System.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicController : ControllerBase {
        private readonly HealthcareAppointmentSystemContext _context;
        private readonly IMapper _mapper;

        public ClinicController(HealthcareAppointmentSystemContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        //Get all clinics

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetClinics() {
            List<Clinic> clinics = await _context.Clinics
                .Include(c => c.DoctorClinics)
                .ThenInclude(dc => dc.Doctor)
                .ToListAsync();
            List<ClinicDTO> clinicDTOs = _mapper.Map<List<ClinicDTO>>(clinics);
            return Ok(clinicDTOs);
        }

        // Get clinic details with doctors
        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicDTO>> GetClinic(int id) {
            Clinic clinic = await _context.Clinics
                .Include(c => c.DoctorClinics)
                .ThenInclude(dc => dc.Doctor)
                .FirstOrDefaultAsync(c => c.ClinicId == id);

            if(clinic == null) {
                return NotFound();
            }
            ClinicDTO clinicDTO = _mapper.Map<ClinicDTO>(clinic);
            return Ok(clinicDTO);
        }

        //Add new clinic
        [HttpPost]
        public async Task<ActionResult<ClinicDTO>> CreateClinic(CreateClinicDTO createDto) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);

            }
            Clinic clinic = _mapper.Map<Clinic>(createDto);
            _context.Clinics.Add(clinic);
            await _context.SaveChangesAsync();

            ClinicDTO clinicDTO = _mapper.Map<ClinicDTO>(clinic);
            return CreatedAtAction(nameof(GetClinic), new { id = clinic.ClinicId }, clinicDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClinicDTO>> UpdateClinic(int id, UpdateClinicDTO updateDto) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            Clinic clinic = await _context.Clinics.FindAsync(id);
            if(clinic == null) {
                return NotFound();
            }

            _mapper.Map(updateDto, clinic);
            await _context.SaveChangesAsync();

            ClinicDTO clinicDTO = _mapper.Map<ClinicDTO>(clinic);
            return Ok(clinicDTO);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ClinicDTO>> DeleteClinic(int id) {
            Clinic clinic = await _context.Clinics.FindAsync(id);
            if(clinic == null) {
                return NotFound();
            }

            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();
            ClinicDTO clinicDTO = _mapper.Map<ClinicDTO>(clinic);
            return Ok(clinicDTO);
        }

        // 2. GET /api/clinics/{clinicId}/schedule?date={date}
        [HttpGet("{clinicId}/schedule")]
        public async Task<ActionResult<ClinicScheduleDTO>> GetClinicSchedule(int clinicId, [FromQuery] DateTime? date) {
            // Default to today if no date provided
            DateTime targetDate = date ?? DateTime.Today;

            // Get clinic with doctors and appointments for the specified date
            Clinic clinic = await _context.Clinics
                .Include(c => c.DoctorClinics.Where(dc =>
                    targetDate >= dc.StartDate &&
                    targetDate <= dc.EndDate))
                .ThenInclude(dc => dc.Doctor)
                .ThenInclude(d => d.Appointments.Where(a =>
                    a.StartTime.Date == targetDate.Date &&
                    a.AppointmentStatus != Status.Cancelled))
                .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(c => c.ClinicId == clinicId);

            if (clinic == null) {
                return NotFound("Clinic not found");
            }

            // Build the schedule response
            ClinicScheduleDTO schedule = new ClinicScheduleDTO {
                ClinicId = clinic.ClinicId,
                ClinicName = clinic.Name,
                Date = targetDate,
                OperatingHours = $"{clinic.StartOperatingHours:hh\\:mm} - {clinic.EndOperatingHours:hh\\:mm}",
                DoctorSchedules = new List<DoctorScheduleDTO>()
            };

            // Group appointments by doctor and organize schedule
            foreach (DoctorClinic doctorClinic in clinic.DoctorClinics) {
                Doctor doctor = doctorClinic.Doctor;
                DoctorScheduleDTO doctorSchedule = new DoctorScheduleDTO {
                    DoctorId = doctor.DoctorId,
                    DoctorName = doctor.FullName,
                    Specialty = doctor.DoctorSpecialty.ToString(),
                    Shift = doctorClinic.DoctorShift.ToString(),
                    WorkingHours = $"{doctor.AvailableStart:hh\\:mm} - {doctor.AvailableEnd:hh\\:mm}",
                    Appointments = new List<ScheduledAppointmentDTO>(),
                    TotalAppointments = doctor.Appointments.Count,
                    AvailableSlots = 0 // Will calculate below
                };

                // Add scheduled appointments
                foreach (Appointment appointment in doctor.Appointments.OrderBy(a => a.StartTime)) {
                    doctorSchedule.Appointments.Add(new ScheduledAppointmentDTO {
                        AppointmentId = appointment.AppointmentId,
                        StartTime = appointment.StartTime,
                        EndTime = appointment.EndTime,
                        Duration = appointment.Duration,
                        PatientName = appointment.Patient?.FullName ?? "Unknown Patient",
                        Reason = appointment.Reason.ToString(),
                        Status = appointment.AppointmentStatus.ToString(),
                        Note = appointment.Note
                    });
                }

                // Calculate available slots for the day
                TimeSpan currentTime = doctor.AvailableStart;
                TimeSpan endTime = doctor.AvailableEnd;
                int availableSlots = 0;

                while (currentTime.Add(TimeSpan.FromMinutes(doctor.AppointmentDurationMinutes)) <= endTime) {
                    DateTime slotStart = targetDate.Date.Add(currentTime);
                    DateTime slotEnd = slotStart.AddMinutes(doctor.AppointmentDurationMinutes);

                    // Check if this slot conflicts with existing appointments
                    bool isAvailable = !doctor.Appointments.Any(a =>
                        slotStart < a.EndTime && slotEnd > a.StartTime);

                    // Check if slot is within clinic operating hours
                    bool withinClinicHours = currentTime >= clinic.StartOperatingHours &&
                                           currentTime.Add(TimeSpan.FromMinutes(doctor.AppointmentDurationMinutes)) <= clinic.EndOperatingHours;

                    // Check if slot matches doctor's shift
                    bool matchesShift = (doctorClinic.DoctorShift == Shift.Morning && currentTime.Hours < 12) ||
                                       (doctorClinic.DoctorShift == Shift.Evening && currentTime.Hours >= 12);

                    if (isAvailable && withinClinicHours && matchesShift) {
                        availableSlots++;
                    }

                    currentTime = currentTime.Add(TimeSpan.FromMinutes(doctor.AppointmentDurationMinutes));
                }

                doctorSchedule.AvailableSlots = availableSlots;
                schedule.DoctorSchedules.Add(doctorSchedule);
            }

            // Calculate summary statistics
            schedule.TotalAppointments = schedule.DoctorSchedules.Sum(ds => ds.TotalAppointments);
            schedule.TotalAvailableSlots = schedule.DoctorSchedules.Sum(ds => ds.AvailableSlots);
            schedule.TotalDoctorsWorking = schedule.DoctorSchedules.Count;

            // Separate doctors by shift
            schedule.MorningDoctors = schedule.DoctorSchedules
                .Where(ds => ds.Shift == "Morning")
                .Select(ds => ds.DoctorName)
                .ToList();

            schedule.EveningDoctors = schedule.DoctorSchedules
                .Where(ds => ds.Shift == "Evening")
                .Select(ds => ds.DoctorName)
                .ToList();

            return Ok(schedule);
        }
    }
}
