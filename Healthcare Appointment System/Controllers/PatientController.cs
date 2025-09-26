using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Healthcare_Appointment_System.Models;
using AutoMapper;

namespace Healthcare_Appointment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly HealthcareAppointmentSystemContext _context;
        private readonly IMapper _mapper;

        public PatientsController(HealthcareAppointmentSystemContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients()
        {
            List<Patient> patients = await _context.Patients.ToListAsync();
            List<PatientDTO> result = _mapper.Map<List<PatientDTO>>(patients);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDTO>> GetPatient(int id)
        {
            Patient patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            PatientDTO dto = _mapper.Map<PatientDTO>(patient);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> CreatePatient(CreatePatientDTO dto)
        {
            bool exists = await _context.Patients.AnyAsync(p => p.PatientEmail == dto.PatientEmail);
            if (exists) return Conflict("Patient with this email already exists.");

            Patient patient = _mapper.Map<Patient>(dto);
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, patient);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePatient(int id, UpdatePatientDTO dto)
        {
            Patient patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            _mapper.Map(dto, patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            Patient patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
