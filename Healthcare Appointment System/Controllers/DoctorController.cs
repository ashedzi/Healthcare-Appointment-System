using AutoMapper;
using Healthcare_Appointment_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Healthcare_Appointment_System.Controllers {
    /// <summary>
    /// Controller for managing doctors in the Healthcare Appointment System.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase {
        private readonly IMapper _mapper;
        private readonly HealthcareAppointmentSystemContext _context;

        public DoctorController(HealthcareAppointmentSystemContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all doctors.
        /// </summary>
        /// <returns>A list of <see cref="DoctorDTO"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctors() {
            List<Doctor> doctors = await _context.Doctors.ToListAsync();
            List<DoctorDTO> doctorDTOs = _mapper.Map<List<DoctorDTO>>(doctors);
            return Ok(doctorDTOs);
        }

        /// <summary>
        /// Retrieves a doctor by ID, including their appointments.
        /// </summary>
        /// <param name="id">The doctor ID.</param>
        /// <returns>A <see cref="DoctorDTO"/> representing the doctor.</returns>
        /// <response code="404">If the doctor is not found.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctor(int id) {
            Doctor? doctor = await _context.Doctors
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.DoctorId == id);
            if(doctor == null) {
                return NotFound();
            }
            DoctorDTO doctorDTO = _mapper.Map<DoctorDTO>(doctor);
            return Ok(doctorDTO);
        }

        /// <summary>
        /// Retrieves doctors by specialty.
        /// </summary>
        /// <param name="specialty">The specialty.</param>
        /// <returns>A list of <see cref="DoctorDTO"/>.</returns>
        [HttpGet("specialty/{specialty}")]
        public async Task<ActionResult<List<DoctorDTO>>> GetDoctorsBySpecialty(Specialty specialty) {
            List<Doctor> doctors = await _context.Doctors
                .Where(d => d.DoctorSpecialty == specialty)
                .ToListAsync();

            return Ok(_mapper.Map<List<DoctorDTO>>(doctors));
        }

        /// <summary>
        /// Retrieves a doctor's schedule (appointments) by doctor ID.
        /// </summary>
        /// <param name="id">The doctor ID.</param>
        /// <returns>List of <see cref="AppointmentDTO"/> for the doctor.</returns>
        /// <response code="404">If the doctor is not found.</response>
        [HttpGet("{id}/schedule")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetDoctorSchedule(int id) {
            Doctor doctor = await _context.Doctors
                .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient)
                .Include(d => d.Appointments)
            .ThenInclude(a => a.Clinic) 
        .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (doctor == null) {
                return NotFound();
            }
            List<AppointmentDTO> appointments = _mapper.Map<List<AppointmentDTO>>(doctor.Appointments);
            return Ok(appointments);
        }

        /// <summary>
        /// Creates a new doctor.
        /// </summary>
        /// <param name="createDto">The doctor data.</param>
        /// <returns>The newly created doctor.</returns>
        /// <response code="400">If the model is invalid.</response>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> CreateDoctor(CreateDoctorDTO createDto) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            Doctor doctor = _mapper.Map<Doctor>(createDto);
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            DoctorDTO doctorDTO = _mapper.Map<DoctorDTO>(doctor);
            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.DoctorId }, doctorDTO);
        }

        /// <summary>
        /// Updates a doctor by ID.
        /// </summary>
        /// <param name="id">The doctor ID.</param>
        /// <param name="updateDto">Updated doctor data.</param>
        /// <response code="400">If the model is invalid.</response>
        /// <response code="404">If the doctor is not found.</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> UpdateDoctor(int id, UpdateDoctorDTO updateDto) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            Doctor doctor = await _context.Doctors.FindAsync(id);
            if(doctor == null) {
                return NotFound();
            }

            _mapper.Map(updateDto, doctor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a doctor by ID.
        /// </summary>
        /// <param name="id">The doctor ID.</param>
        /// <response code="404">If the doctor is not found.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id) {
            Doctor doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) {
                return NotFound();
            }
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Retrieves available slots for a doctor on a given date.
        /// </summary>
        /// <param name="doctorId">The doctor ID.</param>
        /// <param name="date">Optional date (defaults to today).</param>
        /// <returns>A list of <see cref="AvailableSlotDTO"/>.</returns>
        [HttpGet("{doctorId}/available-slots")]
        public async Task<ActionResult<IEnumerable<AvailableSlotDTO>>> GetAvailableSlots(int doctorId, [FromQuery] DateTime? date) {
            DateTime targetDate = date ?? DateTime.Today;
            Doctor doctor = await _context.Doctors
                .Include(d => d.Appointments.Where(a => a.StartTime.Date == targetDate.Date && a.AppointmentStatus != Status.Cancelled))
                .Include(d => d.DoctorClinics.Where(dc => targetDate >= dc.StartDate && targetDate <= dc.EndDate))
                .ThenInclude(dc => dc.Clinic)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);

            if (doctor == null) {
                return NotFound("Doctor not found");
            }

            DoctorClinic doctorClinicForDate = doctor.DoctorClinics.FirstOrDefault();
            if (doctorClinicForDate == null) {
                return Ok(new List<AvailableSlotDTO>()); 
            }

            List<AvailableSlotDTO> availableSlots = new List<AvailableSlotDTO>();

            TimeSpan currentTime = doctor.AvailableStart;
            TimeSpan endTime = doctor.AvailableEnd;

            while (currentTime.Add(TimeSpan.FromMinutes(doctor.AppointmentDurationMinutes)) <= endTime) {
                DateTime slotStart = targetDate.Date.Add(currentTime);
                DateTime slotEnd = slotStart.AddMinutes(doctor.AppointmentDurationMinutes);

                bool isAvailable = !doctor.Appointments.Any(a =>
                    slotStart < a.EndTime && slotEnd > a.StartTime);

                Clinic clinic = doctorClinicForDate.Clinic;
                bool withinClinicHours = currentTime >= clinic.StartOperatingHours &&
                                       currentTime.Add(TimeSpan.FromMinutes(doctor.AppointmentDurationMinutes)) <= clinic.EndOperatingHours;

                bool matchesShift = (doctorClinicForDate.DoctorShift == Shift.Morning && currentTime.Hours < 12) ||
                                   (doctorClinicForDate.DoctorShift == Shift.Evening && currentTime.Hours >= 12);

                if (isAvailable && withinClinicHours && matchesShift) {
                    availableSlots.Add(new AvailableSlotDTO {
                        StartTime = slotStart,
                        EndTime = slotEnd,
                        Duration = doctor.AppointmentDurationMinutes,
                        ClinicId = doctorClinicForDate.ClinicId,
                        ClinicName = clinic.Name
                    });
                }

                currentTime = currentTime.Add(TimeSpan.FromMinutes(doctor.AppointmentDurationMinutes));
            }

            return Ok(availableSlots);
        }

        /// <summary>
        /// Assigns a doctor to a clinic for a specific shift and date range.
        /// </summary>
        /// <param name="doctorId">The doctor ID.</param>
        /// <param name="assignDto">The assignment details.</param>
        /// <response code="201">Assignment created successfully.</response>
        /// <response code="400">If the model is invalid or dates are incorrect.</response>
        /// <response code="404">If the doctor or clinic is not found.</response>
        /// <response code="409">If the doctor is already assigned in conflicting dates.</response>
        [HttpPost("{doctorId}/assign-clinic")]
        public async Task<ActionResult> AssignDoctorToClinic(int doctorId, AssignDoctorToClinicDTO assignDto) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            bool doctorExists = await _context.Doctors.AnyAsync(d => d.DoctorId == doctorId);
            if (!doctorExists) {
                return NotFound("Doctor not found");
            }

            bool clinicExists = await _context.Clinics.AnyAsync(c => c.ClinicId == assignDto.ClinicId);
            if (!clinicExists) {
                return NotFound("Clinic not found");
            }

            bool conflictExists = await _context.DoctorClinics.AnyAsync(dc =>
                dc.DoctorId == doctorId &&
                dc.ClinicId == assignDto.ClinicId &&
                assignDto.StartDate < dc.EndDate &&
                assignDto.EndDate > dc.StartDate);

            if (conflictExists) {
                return Conflict("Doctor is already assigned to this clinic during overlapping dates");
            }

            bool shiftConflict = await _context.DoctorClinics.AnyAsync(dc =>
                dc.DoctorId == doctorId &&
                dc.DoctorShift == assignDto.DoctorShift &&
                assignDto.StartDate < dc.EndDate &&
                assignDto.EndDate > dc.StartDate);

            if (shiftConflict) {
                return Conflict($"Doctor is already assigned to another clinic during {assignDto.DoctorShift} shift for overlapping dates");
            }

            if (assignDto.StartDate >= assignDto.EndDate) {
                return BadRequest("Start date must be before end date");
            }

            if (assignDto.StartDate < DateTime.Today) {
                return BadRequest("Start date cannot be in the past");
            }

            DoctorClinic assignment = new DoctorClinic {
                DoctorId = doctorId,
                ClinicId = assignDto.ClinicId,
                StartDate = assignDto.StartDate,
                EndDate = assignDto.EndDate,
                DoctorShift = assignDto.DoctorShift
            };

            _context.DoctorClinics.Add(assignment);
            await _context.SaveChangesAsync();

            DoctorClinic result = await _context.DoctorClinics
                .Include(dc => dc.Doctor)
                .Include(dc => dc.Clinic)
                .FirstOrDefaultAsync(dc => dc.DoctorId == doctorId &&
                                           dc.ClinicId == assignDto.ClinicId &&
                                           dc.StartDate == assignDto.StartDate);

            DoctorClinicAssignmentDTO response = new DoctorClinicAssignmentDTO {
                DoctorId = result.DoctorId,
                DoctorName = result.Doctor.FullName,
                ClinicId = result.ClinicId,
                ClinicName = result.Clinic.Name,
                StartDate = result.StartDate,
                EndDate = result.EndDate,
                DoctorShift = result.DoctorShift
            };

            return CreatedAtAction(nameof(GetDoctor), new { id = doctorId }, response);
        }
    }
}
