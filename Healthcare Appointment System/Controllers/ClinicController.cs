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

        //    o   POST /api/clinics - Add new clinic

        public IActionResult Index() {
            return View();
        }
    }
}
