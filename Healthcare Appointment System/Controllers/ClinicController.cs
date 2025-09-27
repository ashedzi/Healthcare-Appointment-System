using AutoMapper;
using Healthcare_Appointment_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Healthcare_Appointment_System.Controllers {
    /// <summary>
    /// Controller for managing clinics in the Healthcare Appointment System.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicController : ControllerBase {
        private readonly HealthcareAppointmentSystemContext _context;
        private readonly IMapper _mapper;

        public ClinicController(HealthcareAppointmentSystemContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all clinics.
        /// </summary>
        /// <returns>A list of <see cref="ClinicDTO"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicDTO>>> GetClinics() {
            List<Clinic> clinics = await _context.Clinics
                .Include(c => c.DoctorClinics)
                .ThenInclude(dc => dc.Doctor)
                .ToListAsync();
            List<ClinicDTO> clinicDTOs = _mapper.Map<List<ClinicDTO>>(clinics);
            return Ok(clinicDTOs);
        }

        /// <summary>
        /// Retrieves a clinic by ID, including its doctors.
        /// </summary>
        /// <param name="id">The clinic ID.</param>
        /// <returns>A <see cref="ClinicDTO"/> representing the clinic.</returns>
        /// <response code="404">If the clinic is not found.</response>
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

        /// <summary>
        /// Creates a new clinic.
        /// </summary>
        /// <param name="createDto">The clinic data.</param>
        /// <returns>The newly created clinic.</returns>
        /// <response code="400">If the model is invalid.</response>
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

        /// <summary>
        /// Updates an existing clinic.
        /// </summary>
        /// <param name="id">The clinic ID.</param>
        /// <param name="updateDto">Updated clinic data.</param>
        /// <response code="400">If the model is invalid.</response>
        /// <response code="404">If the clinic is not found.</response>
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

        /// <summary>
        /// Deletes a clinic.
        /// </summary>
        /// <param name="id">The clinic ID.</param>
        /// <response code="404">If the clinic is not found.</response>
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

        /// <summary>
        /// Retrieves the schedule of a clinic for a specific date.
        /// </summary>
        /// <param name="clinicId">The clinic ID.</param>
        /// <param name="date">Optional date (defaults to today).</param>
        /// <returns>A <see cref="ClinicScheduleDTO"/> representing the schedule.</returns>
        [HttpGet("{clinicId}/schedule")]
        public async Task<ActionResult<ClinicScheduleDTO>> GetClinicSchedule(int clinicId, [FromQuery] DateTime? date) {
            DateTime targetDate = date ?? DateTime.Today;
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

            ClinicScheduleDTO schedule = new ClinicScheduleDTO {
                ClinicId = clinic.ClinicId,
                ClinicName = clinic.Name,
                Date = targetDate,
                OperatingHours = $"{clinic.StartOperatingHours:hh\\:mm} - {clinic.EndOperatingHours:hh\\:mm}",
                DoctorSchedules = new List<DoctorScheduleDTO>()
            };

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
                    AvailableSlots = 0 
                };

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

                TimeSpan currentTime = doctor.AvailableStart;
                TimeSpan endTime = doctor.AvailableEnd;
                int availableSlots = 0;

                while (currentTime.Add(TimeSpan.FromMinutes(doctor.AppointmentDurationMinutes)) <= endTime) {
                    DateTime slotStart = targetDate.Date.Add(currentTime);
                    DateTime slotEnd = slotStart.AddMinutes(doctor.AppointmentDurationMinutes);
                    bool isAvailable = !doctor.Appointments.Any(a =>
                        slotStart < a.EndTime && slotEnd > a.StartTime);

                    bool withinClinicHours = currentTime >= clinic.StartOperatingHours &&
                                           currentTime.Add(TimeSpan.FromMinutes(doctor.AppointmentDurationMinutes)) <= clinic.EndOperatingHours;

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

            schedule.TotalAppointments = schedule.DoctorSchedules.Sum(ds => ds.TotalAppointments);
            schedule.TotalAvailableSlots = schedule.DoctorSchedules.Sum(ds => ds.AvailableSlots);
            schedule.TotalDoctorsWorking = schedule.DoctorSchedules.Count;

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
