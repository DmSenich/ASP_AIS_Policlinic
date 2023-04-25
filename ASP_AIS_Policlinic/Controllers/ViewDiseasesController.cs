using ASP_AIS_Policlinic.Areas.Identity.Data;
using ASP_AIS_Policlinic.Models;
using ASP_AIS_Policlinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_AIS_Policlinic.Controllers
{
    [Authorize]
    public class ViewDiseasesController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<PoliclinicUser> _userManager;

        public ViewDiseasesController(AppDBContext context, UserManager<PoliclinicUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (_context.Patients == null)
            {
                return NotFound();
            }
            if (!User.IsInRole("guest"))
            {
                return View(await _context.Patients.ToListAsync());
            }
            if (id == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(ListDiseases), new { id }); 
        }
        public async Task<IActionResult> ListDiseases(int? id)
        {
            if (id == null || _context.Visitings == null)
            {
                return NotFound();
            }

            var diseases = await _context.Diseases.Include(d => d.Visiting).Where(d=>d.Visiting.PatientId == id).ToListAsync();

            //var visitings = await _context.Visitings
            //    .Include(v => v.Doctor)
            //    .Include(v => v.Patient).Include(v => v.Diseases)
            //    .Where(v => v.PatientId == id).ToListAsync();
            //List<VisitingDiseaseViewModel> viewModels = new List<VisitingDiseaseViewModel>();
            //foreach (var visiting in visitings)
            //{
            //    var diseases = await _context.Diseases.Where(d => d.VisitingId == visiting.Id).ToListAsync();
            //    foreach(var disease in diseases)
            //    {
            //        VisitingDiseaseViewModel model = new VisitingDiseaseViewModel();
            //        model.Visiting = visiting;
            //        model.Disease = disease;
            //        viewModels.Add(model);
            //    }
            //}

            return View(diseases);//Model DiseaseAndVisitingViewModel
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Diseases == null)
            {
                return NotFound();
            }

            var disease = await _context.Diseases
                .Include(d => d.DiseaseType).Include(d => d.Visiting).Include(d => d.Visiting.Doctor).Include(d => d.Visiting.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            VisitingDiseaseViewModel model = new VisitingDiseaseViewModel();
            model.Disease = disease;
            model.Visiting = disease.Visiting;
            model.Doctor = disease.Visiting.Doctor;
            model.Patient = disease.Visiting.Patient;


            return View(model);
        }


    }
}
