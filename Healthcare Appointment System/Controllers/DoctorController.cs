using AutoMapper;
using Healthcare_Appointment_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Healthcare_Appointment_System.Controllers {
    [Route("api/[controller")]
    [ApiController]
    public class DoctorController : Controller {
        private readonly IMapper _mapper;
        private readonly HealthcareAppointmentSystemContext _context;

        public DoctorController(HealthcareAppointmentSystemContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctors() {
            List<Doctor> doctors = await _context.Doctors.ToListAsync();
            return Ok(doctors);
        }


        //GET /api/doctors - Get all doctors (with specialty filtering)

        // GET /api/doctors/{id}- Get doctor by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctor(int id) {
            Doctor? doctor = await _context.Doctors.FindAsync(id);
            if(doctor == null) {
                return NotFound();
            }
            return Ok(doctor);
        }

        // POST /api/doctors - Add new doctor
        [HttpPost]
        public async Task<IActionResult> CreateDoctor(Doctor doctor) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.DoctorId }, doctor);
        }

        // PUT /api/doctors/{id}-Update doctor information

        [HttpGet("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, Doctor doctor) {
            if(id != doctor.DoctorId) {
                return BadRequest();
            }
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(doctor).State = EntityState.Modified;
            try {
                await _context.SaveChangesAsync();
            } catch(DbUpdateConcurrencyException) {
                if(!_context.Doctors.Any(d => d.DoctorId == id)) {
                    return NotFound();
                } else {
                    throw;
                }
                return NoContent();
            }
        }
        //o	GET /api/doctors/{id}/ schedule - Get doctor's availability
        //o	GET /api/doctors/specialties - Get all specialties
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id) {
            Doctor? doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) {
                return NotFound();
            }
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
