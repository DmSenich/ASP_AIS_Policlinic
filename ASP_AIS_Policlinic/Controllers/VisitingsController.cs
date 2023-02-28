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

            return View(visiting);
        }

        // GET: Visitings/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "FirstName");
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FirstName");
            return View();
        }

        // POST: Visitings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,PatientId,DateVisiting")] Visiting visiting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(visiting);
                await _context.SaveChangesAsync();
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
