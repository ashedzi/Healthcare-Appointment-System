using AutoMapper;
using Healthcare_Appointment_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Healthcare_Appointment_System.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicController : Controller {
        private readonly HealthcareAppointmentSystemContext _context;
        private readonly IMapper _mapper;

        public ClinicController(HealthcareAppointmentSystemContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        //Get all clinics

        [HttpGet]
        public async Task<IActionResult> GetClinics() {
            List<Clinic> clinics = await _context.Clinics.ToListAsync();
            List<ClinicDTO> clinicDTOs = _mapper.Map<List<ClinicDTO>>(clinics);
            return Ok(clinicDTOs);
        }

        //    o GET /api/clinics/{id} - Get clinic details with doctors
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClinic(int id) {
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

        //    o   POST /api/clinics - Add new clinic
        [HttpPost]
        public async Task<IActionResult> CreateClinic(CreateClinicDTO createDto) {
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
        public async Task<IActionResult> UpdateClinic(int id, UpdateClinicDTO updateDto) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            Clinic clinic = await _context.Clinics.FindAsync(id);
            if(clinic == null) {
                return NotFound();
            }

            _mapper.Map(updateDto, clinic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(int id) {
            Clinic clinic = await _context.Clinics.FindAsync(id);
            if(clinic == null) {
                return NotFound();
            }

            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
