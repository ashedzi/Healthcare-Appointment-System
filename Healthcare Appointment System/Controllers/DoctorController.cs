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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctor(int id) {
            Doctor? doctor = await _context.Doctors.FindAsync(id);
            if(doctor == null) {
                return NotFound();
            }
            return Ok(doctor);
        }

        //GET /api/doctors - Get all doctors (with specialty filtering)
        // GET /api/doctors/{id}- Get doctor by ID
        // POST /api/doctors - Add new doctor
        // PUT /api/doctors/{id}-Update doctor information
        //o	GET /api/doctors/{id}/ schedule - Get doctor's availability
        //o	GET /api/doctors/specialties - Get all specialties

        public IActionResult Index() {
            return View();
        }
    }
}
