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
    public class DiseaseTypesController : Controller
    {
        private readonly AppDBContext _context;

        public DiseaseTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: DiseaseTypes
        public async Task<IActionResult> Index()
        {
              return View(await _context.DiseaseTypes.ToListAsync());
        }

        // GET: DiseaseTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DiseaseTypes == null)
            {
                return NotFound();
            }

            var diseaseType = await _context.DiseaseTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diseaseType == null)
            {
                return NotFound();
            }

            return View(diseaseType);
        }

        // GET: DiseaseTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DiseaseTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NameDisease")] DiseaseType diseaseType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diseaseType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(diseaseType);
        }

        // GET: DiseaseTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DiseaseTypes == null)
            {
                return NotFound();
            }

            var diseaseType = await _context.DiseaseTypes.FindAsync(id);
            if (diseaseType == null)
            {
                return NotFound();
            }
            return View(diseaseType);
        }

        // POST: DiseaseTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NameDisease")] DiseaseType diseaseType)
        {
            if (id != diseaseType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diseaseType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiseaseTypeExists(diseaseType.Id))
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
            return View(diseaseType);
        }

        // GET: DiseaseTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DiseaseTypes == null)
            {
                return NotFound();
            }

            var diseaseType = await _context.DiseaseTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diseaseType == null)
            {
                return NotFound();
            }

            return View(diseaseType);
        }

        // POST: DiseaseTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DiseaseTypes == null)
            {
                return Problem("Entity set 'AppDBContext.DiseaseType'  is null.");
            }
            var diseaseType = await _context.DiseaseTypes.FindAsync(id);
            if (diseaseType != null)
            {
                _context.DiseaseTypes.Remove(diseaseType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiseaseTypeExists(int id)
        {
          return _context.DiseaseTypes.Any(e => e.Id == id);
        }
    }
}
