using ASP_AIS_Policlinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_AIS_Policlinic.Controllers
{
    public class RecordDiagnosisController : Controller
    {
        private readonly AppDBContext _context;

        public RecordDiagnosisController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Patients.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Index(int patientId)
        {
            Patient patient = await _context.Patients.FindAsync(patientId);
            if(patient == null)
            {
                return NotFound();
            }
            ViewBag.PatientId = patientId;
            ViewBag.Patient = patient;
            return View("ChooseDoctor");
        }
        [HttpPost]
        public async Task<IActionResult> ChooseDoctor(int doctorId)
        {
            Doctor doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewBag.DoctorId = doctorId;
            ViewBag.Doctor = doctor;
            return View("ChooseDate");
        }

        [HttpPost]
        public async Task<IActionResult> ChooseDate(int patientId, int doctorId)
        {
            Doctor doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewBag.DoctorId = doctorId;
            ViewBag.Doctor = doctor;
            return View("ChooseDate");
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

    }
}
