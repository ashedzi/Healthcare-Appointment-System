using AutoMapper;
using Healthcare_Appointment_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Healthcare_Appointment_System.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase {
        private readonly IMapper _mapper;
        private readonly HealthcareAppointmentSystemContext _context;

        public DoctorController(HealthcareAppointmentSystemContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctors() {
            List<Doctor> doctors = await _context.Doctors.ToListAsync();
            List<DoctorDTO> doctorDTOs = _mapper.Map<List<DoctorDTO>>(doctors);
            return Ok(doctorDTOs);
        }

        //Get doctor by ID
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

        //Get all docs by specialties
        [HttpGet("specialty/{specialty}")]
        public async Task<ActionResult<List<DoctorDTO>>> GetDoctorsBySpecialty(Specialty specialty) {
            List<Doctor> doctors = await _context.Doctors
                .Where(d => d.DoctorSpecialty == specialty)
                .ToListAsync();

            return Ok(_mapper.Map<List<DoctorDTO>>(doctors));
        }

        //Get doctors appointment availabilty
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

        //Add new doctor
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

        //Update doctor information
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

        //delete doctor
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

        // Add these methods to your existing DoctorController class

        // 1. GET /api/doctors/{doctorId}/available-slots?date={date}
        [HttpGet("{doctorId}/available-slots")]
        public async Task<ActionResult<IEnumerable<AvailableSlotDTO>>> GetAvailableSlots(int doctorId, [FromQuery] DateTime? date) {
            // Default to today if no date provided
            DateTime targetDate = date ?? DateTime.Today;

            // Get doctor with their appointments for the specified date
            Doctor doctor = await _context.Doctors
                .Include(d => d.Appointments.Where(a => a.StartTime.Date == targetDate.Date && a.AppointmentStatus != Status.Cancelled))
                .Include(d => d.DoctorClinics.Where(dc => targetDate >= dc.StartDate && targetDate <= dc.EndDate))
                .ThenInclude(dc => dc.Clinic)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);

            if (doctor == null) {
                return NotFound("Doctor not found");
            }

            // Check if doctor works on this date
            DoctorClinic doctorClinicForDate = doctor.DoctorClinics.FirstOrDefault();
            if (doctorClinicForDate == null) {
                return Ok(new List<AvailableSlotDTO>()); // No available slots if doctor doesn't work
            }

            // Generate time slots based on doctor's availability and appointment duration
            List<AvailableSlotDTO> availableSlots = new List<AvailableSlotDTO>();

            TimeSpan currentTime = doctor.AvailableStart;
            TimeSpan endTime = doctor.AvailableEnd;

            while (currentTime.Add(TimeSpan.FromMinutes(doctor.AppointmentDurationMinutes)) <= endTime) {
                DateTime slotStart = targetDate.Date.Add(currentTime);
                DateTime slotEnd = slotStart.AddMinutes(doctor.AppointmentDurationMinutes);

                // Check if this slot conflicts with existing appointments
                bool isAvailable = !doctor.Appointments.Any(a =>
                    slotStart < a.EndTime && slotEnd > a.StartTime);

                // Check if slot is within clinic operating hours
                Clinic clinic = doctorClinicForDate.Clinic;
                bool withinClinicHours = currentTime >= clinic.StartOperatingHours &&
                                       currentTime.Add(TimeSpan.FromMinutes(doctor.AppointmentDurationMinutes)) <= clinic.EndOperatingHours;

                // Check if slot matches doctor's shift
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

        // 6. POST /api/doctors/{doctorId}/assign-clinic
        [HttpPost("{doctorId}/assign-clinic")]
        public async Task<ActionResult> AssignDoctorToClinic(int doctorId, AssignDoctorToClinicDTO assignDto) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            // Validate doctor exists
            bool doctorExists = await _context.Doctors.AnyAsync(d => d.DoctorId == doctorId);
            if (!doctorExists) {
                return NotFound("Doctor not found");
            }

            // Validate clinic exists
            bool clinicExists = await _context.Clinics.AnyAsync(c => c.ClinicId == assignDto.ClinicId);
            if (!clinicExists) {
                return NotFound("Clinic not found");
            }

            // Check if assignment already exists for overlapping dates
            bool conflictExists = await _context.DoctorClinics.AnyAsync(dc =>
                dc.DoctorId == doctorId &&
                dc.ClinicId == assignDto.ClinicId &&
                assignDto.StartDate < dc.EndDate &&
                assignDto.EndDate > dc.StartDate);

            if (conflictExists) {
                return Conflict("Doctor is already assigned to this clinic during overlapping dates");
            }

            // Check for scheduling conflicts with other clinics during same shift
            bool shiftConflict = await _context.DoctorClinics.AnyAsync(dc =>
                dc.DoctorId == doctorId &&
                dc.DoctorShift == assignDto.DoctorShift &&
                assignDto.StartDate < dc.EndDate &&
                assignDto.EndDate > dc.StartDate);

            if (shiftConflict) {
                return Conflict($"Doctor is already assigned to another clinic during {assignDto.DoctorShift} shift for overlapping dates");
            }

            // Validate dates
            if (assignDto.StartDate >= assignDto.EndDate) {
                return BadRequest("Start date must be before end date");
            }

            if (assignDto.StartDate < DateTime.Today) {
                return BadRequest("Start date cannot be in the past");
            }

            // Create the assignment
            DoctorClinic assignment = new DoctorClinic {
                DoctorId = doctorId,
                ClinicId = assignDto.ClinicId,
                StartDate = assignDto.StartDate,
                EndDate = assignDto.EndDate,
                DoctorShift = assignDto.DoctorShift
            };

            _context.DoctorClinics.Add(assignment);
            await _context.SaveChangesAsync();

            // Return the created assignment details
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
