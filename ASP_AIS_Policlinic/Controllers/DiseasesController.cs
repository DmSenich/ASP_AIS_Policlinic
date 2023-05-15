using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_AIS_Policlinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ASP_AIS_Policlinic.Areas.Identity.Data;

namespace ASP_AIS_Policlinic.Controllers
{
    [Authorize]

    public class DiseasesController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<PoliclinicUser> _userManager;

        public DiseasesController(AppDBContext context, UserManager<PoliclinicUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Diseases
        [Authorize(Roles = "coach, admin")]
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
        public async Task<IActionResult> Create(int? diseaseTypeId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if(!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }
            if(diseaseTypeId != null)
            {
                ViewBag.diseaseTypeId = diseaseTypeId;
                ViewBag.diseaseType = (await _context.DiseaseTypes.FindAsync(diseaseTypeId)).NameDisease;
            }
            //ViewData["DiseaseTypeId"] = new SelectList(_context.DiseaseTypes, "Id", "NameDisease");
            //ViewData["VisitingId"] = new SelectList(_context.Visitings, "Id", "Id");
            return View();
        }

        // POST: Diseases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Disease disease)
        {
            var diseaseType = await _context.DiseaseTypes.FindAsync(disease.DiseaseTypeId);
            disease.DiseaseType = diseaseType;
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
        public async Task<IActionResult> Edit(int? id, int? diseaseTypeId)
        {
            if (id == null || _context.Diseases == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }
            
            var disease = await _context.Diseases.Include(d => d.Visiting).Include(d => d.DiseaseType).FirstAsync(d => d.Id == id);
            if (diseaseTypeId != null)
            {
                ViewBag.diseaseTypeId = diseaseTypeId;
                ViewBag.diseaseType = (await _context.DiseaseTypes.FindAsync(diseaseTypeId)).NameDisease;
            }
            else
            {
                ViewBag.diseaseTypeId = disease.DiseaseTypeId;
                ViewBag.diseaseType = disease.DiseaseType.NameDisease;
            }

            if (disease == null)
            {
                return NotFound();
            }
            //ViewData["DiseaseTypeId"] = new SelectList(_context.DiseaseTypes, "Id", "NameDisease", disease.DiseaseTypeId);
            //ViewData["VisitingId"] = new SelectList(_context.Visitings, "Id", "Id", disease.VisitingId);
            return View(disease);
        }

        // POST: Diseases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Disease disease)
        {
            if (id != disease.Id)
            {
                return NotFound();
            }
            var visiting = await _context.Visitings.FirstAsync(v => v.Id == disease.VisitingId);
            disease.Visiting = visiting;
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
            //ViewData["DiseaseTypeId"] = new SelectList(_context.DiseaseTypes, "Id", "NameDisease", disease.DiseaseTypeId);
            //ViewData["VisitingId"] = new SelectList(_context.Visitings, "Id", "Id", disease.VisitingId);
            return View(disease);
        }


        public async Task<IActionResult> EditDiseaseType(int? id)
        {
            if (_context.Diseases == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }

            //var disease = await _context.Diseases.Include(d => d.Visiting).Include(d => d.DiseaseType).FirstAsync(d => d.Id == id);
            var diseaseTypes = await _context.DiseaseTypes.ToListAsync();
            
            ViewBag.DiseaseId = id;
            if (diseaseTypes == null)
            {
                return NotFound();
            }

            return View(diseaseTypes);
        }
        [HttpPost]
        public async Task<IActionResult> EditDiseaseType(int? diseaseId, int diseaseTypeId, bool? toCreate)
        {
            
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }

            if (toCreate == true)
            {
                if (diseaseTypeId == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Create), new { diseaseTypeId = diseaseTypeId });
            }
            else
            {
                if (diseaseId == null || diseaseTypeId == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Edit), new { id = diseaseId, diseaseTypeId = diseaseTypeId });
                
            }

            //var disease = await _context.Diseases.Include(d => d.Visiting).Include(d => d.DiseaseType).FirstAsync(d => d.Id == id);
            //Disease disease = await _context.Diseases.Include(d => d.DiseaseType).FirstAsync(d => d.Id == diseaseId);
            //DiseaseType diseaseType = await _context.DiseaseTypes.FirstAsync(d => d.Id == diseaseTypeId);
            //disease.DiseaseType = diseaseType;
            //disease.DiseaseTypeId = diseaseTypeId;


        }

        // GET: Diseases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Diseases == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
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
