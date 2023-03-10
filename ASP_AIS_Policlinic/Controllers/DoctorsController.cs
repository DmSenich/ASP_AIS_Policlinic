using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_AIS_Policlinic.Models;

namespace ASP_AIS_Policlinic.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly AppDBContext _context;

        public DoctorsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
              return View(await _context.Doctors.ToListAsync());
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            ViewBag.Specialties = _context.Specialties.ToList();


            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            ViewBag.Specialties = _context.Specialties.ToList();
            return View();
        }
        public IActionResult Create(bool? fromRecordDiagnosis)
        {
            if (fromRecordDiagnosis == true)
                ViewBag.toRecordDiagnosis = true;
            else
                ViewBag.toRecordDiagnosis = false;
            return View();
        }
        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doctor doctor, int[] selectedSpecialties)
        {
            if (ModelState.IsValid && selectedSpecialties != null)
            {
                foreach(var sp in _context.Specialties.Where(sp0 => selectedSpecialties.Contains(sp0.Id)))
                {
                    doctor.Specialties.Add(sp);
                }
                if (doctor.toRecordDiagnosis == true)
                    return RedirectToAction("ChooseDoctor", "RecordDiagnosis");
                else
                    return RedirectToAction(nameof(Index));

                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewBag.Specialties = _context.Specialties.ToList();
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Doctor doctor, int[] selectedSpecialties)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid && selectedSpecialties != null)
            {
                try
                {
                    
                    //_context.Update(doctor);
                    Doctor newDoc = await _context.Doctors.FindAsync(id);
                    newDoc.Specialties.Clear();
                    foreach(var sp in _context.Specialties)
                    {
                        if(selectedSpecialties.Contains(sp.Id))
                            newDoc.Specialties.Add(sp);
                    }
                    _context.Update(newDoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
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
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Doctors == null)
            {
                return Problem("Entity set 'AppDBContext.Doctors'  is null.");
            }
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
          return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
