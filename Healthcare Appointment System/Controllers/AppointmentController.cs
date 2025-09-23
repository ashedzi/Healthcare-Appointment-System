using Microsoft.AspNetCore.Mvc;

namespace Healthcare_Appointment_System.Controllers {
    public class AppointmentController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
