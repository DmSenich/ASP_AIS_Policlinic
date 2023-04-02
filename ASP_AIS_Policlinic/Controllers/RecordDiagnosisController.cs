using ASP_AIS_Policlinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP_AIS_Policlinic.Controllers
{
    [Authorize]
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
            return View(nameof(ChooseDoctor), await _context.Doctors.ToListAsync());
        }

        public async Task<IActionResult> ChooseDoctor()
        {
            return View(await _context.Doctors.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> ChooseDoctor(int patientId, int doctorId)
        {
            Doctor doctor = await _context.Doctors.FindAsync(doctorId);
            Patient patient = await _context.Patients.FindAsync(patientId);
            if (doctor == null || patient == null)
            {
                return NotFound();
            }
            ViewBag.PatientId = patientId;
            ViewBag.Patient = patient;
            ViewBag.DoctorId = doctorId;
            ViewBag.Doctor = doctor;
            return View(nameof(ChooseDate), await _context.Visitings.ToListAsync());
        }
        public async Task<IActionResult> ChooseDate()
        {
            //ViewBag.Today = DateTime.Now.ToShortDateString();
            return View(await _context.Visitings.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> ChooseDate(int patientId, int doctorId, DateTime visitingDate)
        {
            var visitings = await _context.Visitings.ToListAsync();
            if (visitings == null)
            {
                return NotFound();
            }
            Doctor doctor = _context.Doctors.FirstOrDefault(d => d.Id == doctorId);
            Patient patient = _context.Patients.FirstOrDefault(d => d.Id == patientId);
            ViewBag.Doctor = doctor;
            ViewBag.Patient = patient;
            //ViewBag.DoctorId = doctorId;
            //ViewBag.PatientId = patientId;
            ViewBag.VisitingDate = visitingDate.ToShortDateString();
            if (visitings.Count(v => v.PatientId == patientId && v.DoctorId == doctorId && v.DateVisiting == visitingDate) > 0) { return View(visitings); }
            
            return View(nameof(ConfirmRecord));
        }

        //public async Task<IActionResult> ConfirmRecord()
        //{
        //    //ViewBag.Today = DateTime.Now.ToShortDateString();
        //    return View();
        //}
        [HttpPost]
        public async Task<IActionResult> ConfirmRecord([Bind("PatientId,DoctorId,DateVisiting")] Visiting visiting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(visiting);
                await _context.SaveChangesAsync();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "FirstName", visiting.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FirstName", visiting.PatientId);
            return RedirectToAction(nameof(Index), "Home");
        }
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Patients == null)
        //    {
        //        return NotFound();
        //    }

        //    var patient = await _context.Patients
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (patient == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(patient);
        //}

    }
}
