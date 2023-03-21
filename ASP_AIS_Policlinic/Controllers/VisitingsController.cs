using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_AIS_Policlinic.Models;
using System.Numerics;
using ASP_AIS_Policlinic.Models.ViewModels;

namespace ASP_AIS_Policlinic.Controllers
{
    public class VisitingsController : Controller
    {
        private readonly AppDBContext _context;

        public VisitingsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Visitings
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.Visitings.Include(v => v.Doctor).Include(v => v.Patient);
            return View(await appDBContext.ToListAsync());
        }

        // GET: Visitings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Visitings == null)
            {
                return NotFound();
            }

            var visiting = await _context.Visitings
                .Include(v => v.Doctor)
                .Include(v => v.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visiting == null)
            {
                return NotFound();
            }
            VisitingDetailsViewModel viewModel = new VisitingDetailsViewModel();
            viewModel.Visiting = visiting;
            viewModel.Diseases = _context.Diseases.Include(d=>d.DiseaseType).Where(d => d.VisitingId == id);
            return View(viewModel);
        }

        public IActionResult AddDiseaseToVisiting(int id)
        {
            ViewBag.VisitingId = id;
            return View(_context.Diseases.Where(v => v.VisitingId == null));
        }

        [HttpPost]
        public async Task<IActionResult> AddDiseaseToVisiting(int visitingId, int diseaseId)
        {
            Disease disease = _context.Diseases.Find(diseaseId);
            disease.VisitingId = visitingId;
            disease.Visiting = _context.Visitings.Find(visitingId);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = visitingId });
        }

        // GET: Visitings/Create
        public IActionResult Create(bool? fromRecordDiagnosis)
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "FirstName");
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FirstName");
            if (fromRecordDiagnosis == true)
                ViewBag.toRecordDiagnosis = true;
            else
                ViewBag.toRecordDiagnosis = false;
            return View();
        }
        // POST: Visitings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,PatientId,DateVisiting,toRecordDiagnosis")] Visiting visiting)
        {
            if (ModelState.IsValid)
            {
 
                _context.Add(visiting);
                await _context.SaveChangesAsync();

                if (visiting.toRecordDiagnosis == true)
                    return RedirectToAction("ChooseDate", "RecordDiagnosis");
                else
                    return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "FirstName", visiting.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FirstName", visiting.PatientId);
            return View(visiting);
        }

        // GET: Visitings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Visitings == null)
            {
                return NotFound();
            }

            var visiting = await _context.Visitings.FindAsync(id);
            if (visiting == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "FirstName", visiting.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FirstName", visiting.PatientId);
            return View(visiting);
        }

        // POST: Visitings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,PatientId,DateVisiting")] Visiting visiting)
        {
            if (id != visiting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visiting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitingExists(visiting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "FirstName", visiting.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FirstName", visiting.PatientId);
            return View(visiting);
        }

        // GET: Visitings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Visitings == null)
            {
                return NotFound();
            }

            var visiting = await _context.Visitings
                .Include(v => v.Doctor)
                .Include(v => v.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visiting == null)
            {
                return NotFound();
            }

            return View(visiting);
        }

        // POST: Visitings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Visitings == null)
            {
                return Problem("Entity set 'AppDBContext.Visitings'  is null.");
            }
            var visiting = await _context.Visitings.FindAsync(id);
            if (visiting != null)
            {
                _context.Visitings.Remove(visiting);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitingExists(int id)
        {
          return _context.Visitings.Any(e => e.Id == id);
        }
    }
}
