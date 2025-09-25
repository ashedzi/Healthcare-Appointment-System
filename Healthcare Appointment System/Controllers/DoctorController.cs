using Healthcare_Appointment_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Healthcare_Appointment_System.Controllers {
    [Route("api/[controller")]
    [ApiController]
    public class DoctorController : ControllerBase {
        private readonly HealthcareAppointmentSystemContext _context;

        public DoctorController(HealthcareAppointmentSystemContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctors() {
            List<Doctor> doctors = await _context.Doctors.ToListAsync();
            return Ok(doctors);
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
