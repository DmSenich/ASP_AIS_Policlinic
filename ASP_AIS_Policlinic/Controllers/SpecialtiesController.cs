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
using ASP_AIS_Policlinic.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;

namespace ASP_AIS_Policlinic.Controllers
{
    [Authorize]
    public class SpecialtiesController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<PoliclinicUser> _userManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public SpecialtiesController(AppDBContext context, UserManager<PoliclinicUser> userManager, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
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

        public async Task<IActionResult> AddDoctorToSpecialty(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }
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
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }
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
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
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
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
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

        
        public FileResult? GetReportAboutDoctor(int? id)
        {
            if (id == null || _context.Specialties == null)
            {
                return null;
            }
            // Путь к файлу с шаблоном
            string path = "/Reports/templates/report_template_of_specialtyDoctors.xlsx";
            //Путь к файлу с результатом
            string result = "/Reports/report_specialties.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Сеничкин Д.О.";
                excelPackage.Workbook.Properties.Title = "Список врачей по специальностям";
                excelPackage.Workbook.Properties.Subject = "Врачи";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //плучаем лист по имени.
                ExcelWorksheet worksheet =
                    excelPackage.Workbook.Worksheets["Doctors"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Doctor> Doctors = _context.Doctors.Include(d => d.Specialties).ToList();
                worksheet.Cells[startLine, 7].Value = _context.Specialties.Where(sp => sp.Id == id).First().NameSpecialty;
                foreach (Doctor doctor in Doctors)
                {
                    if(doctor.Specialties.Contains(_context.Specialties.Where(sp => sp.Id == id).First()))
                    {
                        worksheet.Cells[startLine, 1].Value = startLine - 2;
                        worksheet.Cells[startLine, 2].Value = doctor.Id;
                        worksheet.Cells[startLine, 3].Value = doctor.LastName;
                        worksheet.Cells[startLine, 4].Value = doctor.FirstName;
                        worksheet.Cells[startLine, 5].Value = doctor.Patronymic;
                        worksheet.Cells[startLine, 6].Value = doctor.WorkExperience;
                        startLine++;
                    }                   
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
                // Тип файла - content-type
                string file_type =
                    "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
                // Имя файла - необязательно
                string file_name = "report_specialties.xlsx";
                return File(result, file_type, file_name);
            }
        }
        public FileResult? GetReportAboutDoctorAll()
        {
            if (_context.Specialties == null)
            {
                return null;
            }
            // Путь к файлу с шаблоном
            string path = "/Reports/templates/report_template_of_specialtyDoctors.xlsx";
            //Путь к файлу с результатом
            string result = "/Reports/report_specialtiesAll.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Сеничкин Д.О.";
                excelPackage.Workbook.Properties.Title = "Список врачей по специальностям";
                excelPackage.Workbook.Properties.Subject = "Врачи";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //плучаем лист по имени.
                ExcelWorksheet worksheet =
                    excelPackage.Workbook.Worksheets["Doctors"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Doctor> Doctors = _context.Doctors.Include(d => d.Specialties).ToList();
                List<Specialty> Specialties = _context.Specialties.Include(sp => sp.Doctors).ToList();
               

                foreach(Specialty specialty in Specialties)
                {
                    worksheet.Cells[startLine, 7].Value = specialty.NameSpecialty;
                    foreach (Doctor doctor in Doctors)
                    {
                        if (doctor.Specialties.Contains(specialty))
                        {
                            worksheet.Cells[startLine, 1].Value = startLine - 2;
                            worksheet.Cells[startLine, 2].Value = doctor.Id;
                            worksheet.Cells[startLine, 3].Value = doctor.LastName;
                            worksheet.Cells[startLine, 4].Value = doctor.FirstName;
                            worksheet.Cells[startLine, 5].Value = doctor.Patronymic;
                            worksheet.Cells[startLine, 6].Value = doctor.WorkExperience;
                            startLine++;
                        }
                    }
                }
                
                //созраняем в новое место
                excelPackage.SaveAs(fr);
                // Тип файла - content-type
                string file_type =
                    "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
                // Имя файла - необязательно
                string file_name = "report_specialtiesAll.xlsx";
                return File(result, file_type, file_name);
            }
        }
    }
}
