using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Healthcare_Appointment_System.Models;
using AutoMapper;

namespace Healthcare_Appointment_System.Controllers
{
    /// <summary>
    /// Controller for managing appointments in the Healthcare Appointment System.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly HealthcareAppointmentSystemContext _context;
        private readonly IMapper _mapper;

        public AppointmentsController(HealthcareAppointmentSystemContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all appointments, optionally filtered by date and status.
        /// </summary>
        /// <param name="date">Optional date to filter appointments.</param>
        /// <param name="status">Optional status to filter appointments.</param>
        /// <returns>A list of <see cref="AppointmentDTO"/>.</returns>
        /// <response code="200">Returns the list of appointments.</response>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointments(DateTime? date, Status? status)
        {
            IQueryable<Appointment> query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic);

            if (date.HasValue)
                query = query.Where(a => a.StartTime.Date == date.Value.Date);

            if (status.HasValue)
                query = query.Where(a => a.AppointmentStatus == status.Value);

            List<Appointment> appointments = await query.ToListAsync();
            List<AppointmentDTO> result = _mapper.Map<List<AppointmentDTO>>(appointments);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific appointment by ID.
        /// </summary>
        /// <param name="id">The ID of the appointment.</param>
        /// <returns>An <see cref="AppointmentDTO"/> representing the appointment.</returns>
        /// <response code="200">Returns the appointment if found.</response>
        /// <response code="404">If the appointment with the specified ID is not found.</response>

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDTO>> GetAppointment(int id)
        {
            Appointment appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null) return NotFound();

            AppointmentDTO dto = _mapper.Map<AppointmentDTO>(appointment);
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new appointment.
        /// </summary>
        /// <param name="dto">The details of the appointment to create.</param>
        /// <returns>The created appointment.</returns>
        /// <response code="201">Returns the newly created appointment.</response>
        /// <response code="404">If the patient, doctor, or clinic does not exist.</response>
        /// <response code="409">If there is a scheduling conflict for the patient.</response>
        [HttpPost]
        public async Task<ActionResult> CreateAppointment(CreateAppointmentDTO dto)
        {
            bool patientExists = await _context.Patients.AnyAsync(p => p.PatientId == dto.PatientId);
            bool doctorExists = await _context.Doctors.AnyAsync(d => d.DoctorId == dto.DoctorId);
            bool clinicExists = await _context.Clinics.AnyAsync(c => c.ClinicId == dto.ClinicId);

            if (!patientExists) return NotFound("Patient not found.");
            if (!doctorExists) return NotFound("Doctor not found.");
            if (!clinicExists) return NotFound("Clinic not found.");

            // Check if doctor works at this clinic
            bool doctorWorksAtClinic = await _context.DoctorClinics.AnyAsync(dc =>
                 dc.DoctorId == dto.DoctorId &&
                 dc.ClinicId == dto.ClinicId &&
                 dto.StartTime.Date >= dc.StartDate.Date &&
                 dto.StartTime.Date <= dc.EndDate.Date &&
                 ((dc.DoctorShift == Shift.Morning && dto.StartTime.Hour < 12) ||
                  (dc.DoctorShift == Shift.Evening && dto.StartTime.Hour >= 12))
             );

            // Check for appointment conflicts
            bool conflict = await _context.Appointments.AnyAsync(a =>
                a.PatientId == dto.PatientId &&
                dto.StartTime < a.EndTime &&
                dto.StartTime.AddMinutes(dto.Duration) > a.StartTime);

            if (conflict) return Conflict("This patient already has an appointment during this time.");

            Appointment appointment = _mapper.Map<Appointment>(dto);
            appointment.AppointmentStatus = Status.Scheduled;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.AppointmentId }, appointment);
        }

        /// <summary>
        /// Updates the status of an existing appointment.
        /// </summary>
        /// <param name="id">The ID of the appointment.</param>
        /// <param name="newStatus">The new status to set.</param>
        /// <response code="204">Status updated successfully.</response>
        /// <response code="404">If the appointment is not found.</response>
        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateAppointmentStatus(int id, Status newStatus)
        {
            Appointment appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.AppointmentStatus = newStatus;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes an appointment.
        /// </summary>
        /// <param name="id">The ID of the appointment to delete.</param>
        /// <response code="204">Appointment deleted successfully.</response>
        /// <response code="404">If the appointment is not found.</response>

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            Appointment appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
