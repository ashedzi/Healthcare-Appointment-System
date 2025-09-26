using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Healthcare_Appointment_System.Models;
using AutoMapper;

namespace Healthcare_Appointment_System.Controllers
{
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


        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateAppointmentStatus(int id, Status newStatus)
        {
            Appointment appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.AppointmentStatus = newStatus;
            await _context.SaveChangesAsync();

            return NoContent();
        }

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
