using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_AIS_Policlinic.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASP_AIS_Policlinic.Controllers
{
    [Authorize]
    public class DiseasesController : Controller
    {
        private readonly AppDBContext _context;

        public DiseasesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Diseases
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.Diseases.Include(d => d.DiseaseType).Include(d => d.Visiting);
            return View(await appDBContext.ToListAsync());
        }

        // GET: Diseases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Diseases == null)
            {
                return NotFound();
            }

            var disease = await _context.Diseases
                .Include(d => d.DiseaseType).Include(d => d.Visiting)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (disease == null)
            {
                return NotFound();
            }

            return View(disease);
        }

        // GET: Diseases/Create
        public IActionResult Create()
        {
            ViewData["DiseaseTypeId"] = new SelectList(_context.DiseaseTypes, "Id", "NameDisease");
            ViewData["VisitingId"] = new SelectList(_context.Visitings, "Id", "Id");
            return View();
        }

        // POST: Diseases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description")] Disease disease)
        {
            if (ModelState.IsValid)
            {
                _context.Add(disease);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["DiseaseTypeId"] = new SelectList(_context.DiseaseTypes, "Id", "NameDisease", disease.DiseaseTypeId);
            //ViewData["VisitingId"] = new SelectList(_context.Visitings, "Id", "Id", disease.VisitingId);
            return View(disease);
        }

        // GET: Diseases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Diseases == null)
            {
                return NotFound();
            }

            var disease = await _context.Diseases.FindAsync(id);
            if (disease == null)
            {
                return NotFound();
            }
            ViewData["DiseaseTypeId"] = new SelectList(_context.DiseaseTypes, "Id", "NameDisease", disease.DiseaseTypeId);
            ViewData["VisitingId"] = new SelectList(_context.Visitings, "Id", "Id", disease.VisitingId);
            return View(disease);
        }

        // POST: Diseases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Description,DiseaseTypeId")] Disease disease)
        {
            if (id != disease.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disease);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiseaseExists(disease.Id))
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
            ViewData["DiseaseTypeId"] = new SelectList(_context.DiseaseTypes, "Id", "NameDisease", disease.DiseaseTypeId);
            ViewData["VisitingId"] = new SelectList(_context.Visitings, "Id", "Id", disease.VisitingId);
            return View(disease);
        }

        // GET: Diseases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Diseases == null)
            {
                return NotFound();
            }

            var disease = await _context.Diseases
                .Include(d => d.DiseaseType).Include(d => d.Visiting)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (disease == null)
            {
                return NotFound();
            }

            return View(disease);
        }

        // POST: Diseases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Diseases == null)
            {
                return Problem("Entity set 'AppDBContext.Disease'  is null.");
            }
            var disease = await _context.Diseases.FindAsync(id);
            if (disease != null)
            {
                _context.Diseases.Remove(disease);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiseaseExists(int id)
        {
          return _context.Diseases.Any(e => e.Id == id);
        }
    }
}
