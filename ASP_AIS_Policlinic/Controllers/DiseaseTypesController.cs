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
using Microsoft.AspNetCore.Identity;
using ASP_AIS_Policlinic.Areas.Identity.Data;

namespace ASP_AIS_Policlinic.Controllers
{
    [Authorize(Roles = "coach, admin")]
    public class DiseaseTypesController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<PoliclinicUser> _userManager;

        public DiseaseTypesController(AppDBContext context, UserManager<PoliclinicUser> userManaher)
        {
            _context = context;
            _userManager = userManaher;
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

            var diseaseType = await _context.DiseaseTypes.Include(dt => dt.Diseases)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diseaseType == null)
            {
                return NotFound();
            }
            DiseaseTypeDetailsViewModel viewModel = new DiseaseTypeDetailsViewModel();
            viewModel.DiseaseType = diseaseType;
            viewModel.Diseases = _context.Diseases.Include(d => d.DiseaseType).Where(d => d.DiseaseTypeId == id);

            return View(viewModel);
        }

        public async Task<IActionResult> AddDiseaseToDiseaseType(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }
            ViewBag.DiseaseTypeId = id;
            return View(_context.Diseases.Include(d => d.DiseaseType).Where(d => d.DiseaseTypeId == null));
        }

        [HttpPost]
        public async Task<IActionResult> AddDiseaseToDiseaseType(int diseaseTypeId, int diseaseId)
        {
            Disease disease = _context.Diseases.Include(d => d.DiseaseType).FirstOrDefault(d => d.Id == diseaseId);
            disease.DiseaseTypeId = diseaseTypeId;
            disease.DiseaseType = _context.DiseaseTypes.FirstOrDefault(dt => dt.Id == diseaseTypeId);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = diseaseTypeId });
        }

        // GET: DiseaseTypes/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }
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
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
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
        public async Task<IActionResult> Edit(int id, DiseaseType diseaseType)
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
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
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
