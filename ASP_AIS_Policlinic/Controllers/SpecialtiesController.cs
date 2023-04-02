using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_AIS_Policlinic.Models;
using ASP_AIS_Policlinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace ASP_AIS_Policlinic.Controllers
{
    [Authorize]
    public class SpecialtiesController : Controller
    {
        private readonly AppDBContext _context;

        public SpecialtiesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Specialties
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.Specialties.Include(sp => sp.Doctors);
            return View(await appDBContext.ToListAsync());
        }

        // GET: Specialties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Specialties == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties.Include(sp => sp.Doctors)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }
            SpecialtyDetailsViewModel viewModel = new SpecialtyDetailsViewModel();
            viewModel.Specialty = specialty;
            viewModel.Doctors = _context.Doctors.Where(d => d.Specialties.FirstOrDefault(sp => sp.Id == id).Id == id);

            return View(viewModel);
        }

        public IActionResult AddDoctorToSpecialty(int id)
        {
            ViewBag.SpecialtyId = id;
            return View(_context.Doctors.Include(d => d.Specialties));
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctorToSpecialty(int doctorId, int specialtyId)
        {
            Specialty specialty = _context.Specialties.Include(sp => sp.Doctors).FirstOrDefault(sp => sp.Id == specialtyId);
            Doctor doctor = _context.Doctors.Include(d => d.Specialties).FirstOrDefault(d => d.Id == doctorId);

            if (doctor.Specialties.Contains(specialty))
            {
                specialty.Doctors.Remove(doctor);
                doctor.Specialties.Remove(specialty);
            }
            else
            {
                specialty.Doctors.Add(doctor);
                doctor.Specialties.Add(specialty);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = specialtyId });
        }

        // GET: Specialties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NameSpecialty")] Specialty specialty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specialty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specialty);
        }

        // GET: Specialties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Specialties == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }
            return View(specialty);
        }

        // POST: Specialties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NameSpecialty")] Specialty specialty)
        {
            if (id != specialty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialtyExists(specialty.Id))
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
            return View(specialty);
        }

        // GET: Specialties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Specialties == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        // POST: Specialties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Specialties == null)
            {
                return Problem("Entity set 'AppDBContext.Specialty'  is null.");
            }
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty != null)
            {
                _context.Specialties.Remove(specialty);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialtyExists(int id)
        {
          return _context.Specialties.Any(e => e.Id == id);
        }
    }
}
