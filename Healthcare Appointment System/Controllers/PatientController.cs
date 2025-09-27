using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Healthcare_Appointment_System.Models;
using AutoMapper;

namespace Healthcare_Appointment_System.Controllers
{
    /// <summary>
    /// Controller for managing patients in the Healthcare Appointment System.
    /// </summary>
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

        /// <summary>
        /// Retrieves all active patients.
        /// </summary>
        /// <returns>A list of <see cref="PatientDTO"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients()
        {
            List<Patient> patients = await _context.Patients
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            List<PatientDTO> result = _mapper.Map<List<PatientDTO>>(patients);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific patient by ID.
        /// </summary>
        /// <param name="id">The patient ID.</param>
        /// <returns>A <see cref="PatientDTO"/> representing the patient.</returns>
        /// <response code="404">If the patient is not found.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDTO>> GetPatient(int id)
        {
            Patient patient = await _context.Patients
                .Where(p => !p.IsDeleted)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null) return NotFound();

            PatientDTO dto = _mapper.Map<PatientDTO>(patient);
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new patient.
        /// </summary>
        /// <param name="dto">The patient data.</param>
        /// <response code="201">Patient created successfully.</response>
        /// <response code="409">If a patient with the same email already exists.</response>
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

        /// <summary>
        /// Updates a patient by ID.
        /// </summary>
        /// <param name="id">The patient ID.</param>
        /// <param name="dto">Updated patient data.</param>
        /// <response code="404">If the patient is not found.</response>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePatient(int id, UpdatePatientDTO dto)
        {
            Patient patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            _mapper.Map(dto, patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Soft deletes a patient by marking as deleted.
        /// </summary>
        /// <param name="id">The patient ID.</param>
        /// <response code="204">Patient deleted successfully.</response>
        /// <response code="400">If the patient is already deleted.</response>
        /// <response code="404">If the patient is not found.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            Patient patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            if (patient.IsDeleted) return BadRequest("Patient already deleted.");

            patient.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
