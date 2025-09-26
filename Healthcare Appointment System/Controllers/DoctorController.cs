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
        [HttpGet("specialties")]
        public ActionResult<IEnumerable<string>> GetSpecialties() {
            List<string> specialties = System.Enum.GetNames(typeof(Specialty)).ToList();
            return Ok(specialties);
        }

        //Get doctors appointment availabilty
        [HttpGet("{id}/schedule")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetDoctorSchedule(int id) {
            Doctor doctor = await _context.Doctors
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            if(doctor == null) {
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
        public async Task<IActionResult> UpdateDoctor(int id, UpdateDoctorDTO updateDto) {
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
    }
}
