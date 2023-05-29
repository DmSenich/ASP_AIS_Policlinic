using ASP_AIS_Policlinic.Areas.Identity.Data;
using ASP_AIS_Policlinic.Models;
using ASP_AIS_Policlinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace ASP_AIS_Policlinic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<PoliclinicUser> _userManager;
        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<PoliclinicUser> userManager, AppDBContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if(HttpContext.User.IsInRole("coach") || HttpContext.User.IsInRole("guest"))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if(user.ModelId != null)
                {
                    if (HttpContext.User.IsInRole("coach"))
                    {
                        if(await _context.Doctors.FirstOrDefaultAsync(d => d.Id == user.ModelId) == null)
                        {
                            return RedirectToAction(nameof(IndexForDoctor));
                        }
                    }
                    if (HttpContext.User.IsInRole("guest"))
                    {
                        if (await _context.Patients.FirstOrDefaultAsync(p => p.Id == user.ModelId) == null)
                        {
                            return RedirectToAction(nameof(IndexForPatient));
                        }
                    }
                }
                if (user.ModelId == null)
                {
                    if (HttpContext.User.IsInRole("coach"))
                    {
                        return RedirectToAction(nameof(IndexForDoctor));
                    }
                    if (HttpContext.User.IsInRole("guest"))
                    {
                        return RedirectToAction(nameof(IndexForPatient));
                    }
                }
            }
            
            //HttpContext.User;
            return View();
        }
        [Authorize(Roles = "coach")]
        public async Task<IActionResult> IndexForDoctor()
        {
            UserDetailsViewModel viewModel = new UserDetailsViewModel();
            viewModel.User = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.UserId = viewModel.User.Id;
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> IndexForDoctor(int userId)
        {
            UserDetailsViewModel viewModel = new UserDetailsViewModel();
            viewModel.User = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.UserId = viewModel.User.Id;
            return View(viewModel);
        }

        [Authorize(Roles = "guest")]
        public async Task<IActionResult> IndexForPatient()
        {
            UserDetailsViewModel viewModel = new UserDetailsViewModel();
            viewModel.User = await _userManager.GetUserAsync(HttpContext.User);
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}