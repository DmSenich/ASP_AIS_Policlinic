using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_AIS_Policlinic.Models;
using System.Numerics;
using ASP_AIS_Policlinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using ASP_AIS_Policlinic.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;

namespace ASP_AIS_Policlinic.Controllers
{
    [Authorize]
    public class VisitingsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<PoliclinicUser> _userManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public VisitingsController(AppDBContext context, UserManager<PoliclinicUser> userManager, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }

        // GET: Visitings
        public async Task<IActionResult> Index(String? dateRange)
        {
            //dates.ToString();
            List<DateTime> dates = new List<DateTime>();
            if (dateRange != null)
            {
                
                foreach(string date in dateRange.Split("-"))
                {
                    dates.Add(DateTime.Parse(date));
                }
                
            }
            ViewBag.DateRange = dateRange;
            if (User.IsInRole("guest"))
            {
                
                var user = await _userManager.GetUserAsync(User);
                if (dates.Count != 0)
                {
                    var appDBContext = _context.Visitings.Include(v => v.Doctor).Include(v => v.Patient).Where(v => v.PatientId == user.ModelId).Where(v => v.DateVisiting >= dates[0] && v.DateVisiting <= dates[1]);
                    return View(await appDBContext.ToListAsync());
                }
                else
                {
                    var appDBContext = _context.Visitings.Include(v => v.Doctor).Include(v => v.Patient).Where(v => v.PatientId == user.ModelId);
                    return View(await appDBContext.ToListAsync());
                }
                
            }
            else
            {
                if (dates.Count != 0)
                {
                    var appDBContext = _context.Visitings.Include(v => v.Doctor).Include(v => v.Patient).Where(v => v.DateVisiting >= dates[0] && v.DateVisiting <= dates[1]);
                    return View(await appDBContext.ToListAsync());
                }
                else
                {
                    var appDBContext = _context.Visitings.Include(v => v.Doctor).Include(v => v.Patient);
                    return View(await appDBContext.ToListAsync());
                }
                
            }      
        }

        // GET: Visitings/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Visitings == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("coach"))
            {
                ViewBag.DoctorId = user.ModelId;
            }
            var visiting = await _context.Visitings
                .Include(v => v.Doctor)
                .Include(v => v.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visiting == null)
            {
                return NotFound();
            }
            if(User.IsInRole("guest") && visiting.PatientId != user.ModelId) 
            {
                return new StatusCodeResult(403);
            }
            VisitingDetailsViewModel viewModel = new VisitingDetailsViewModel();
            viewModel.Visiting = visiting;
            viewModel.Diseases = _context.Diseases.Include(d=>d.DiseaseType).Where(d => d.VisitingId == id);
            return View(viewModel);
        }
        [Authorize(Roles = "coach, admin")]
        public async Task<IActionResult> AddDiseaseToVisiting(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var visiting = _context.Visitings.FirstOrDefault(d => d.Id == id);
            if (!(await _userManager.IsInRoleAsync(user, "coach") && user.ModelId == visiting.DoctorId || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }
            ViewBag.VisitingId = id;
            return View(_context.Diseases.Include(d => d.DiseaseType).Where(v => v.VisitingId == null));
        }

        [HttpPost]
        public async Task<IActionResult> AddDiseaseToVisiting(int visitingId, int diseaseId)
        {
            Disease disease = _context.Diseases.Find(diseaseId);
            disease.VisitingId = visitingId;
            disease.Visiting = _context.Visitings.Find(visitingId);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = visitingId });
        }

        // GET: Visitings/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create(bool? fromRecordDiagnosis)
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "FirstName");
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FirstName");
            if (fromRecordDiagnosis == true)
                ViewBag.toRecordDiagnosis = true;
            else
                ViewBag.toRecordDiagnosis = false;
            return View();
        }
        // POST: Visitings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,PatientId,DateVisiting,toRecordDiagnosis")] Visiting visiting)
        {
            if (ModelState.IsValid)
            {
 
                _context.Add(visiting);
                await _context.SaveChangesAsync();

                if (visiting.toRecordDiagnosis == true)
                    return RedirectToAction("ChooseDate", "RecordDiagnosis");
                else
                    return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "FirstName", visiting.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FirstName", visiting.PatientId);
            return View(visiting);
        }

        // GET: Visitings/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Visitings == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Visitings == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
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
                return Problem("Набор сущностей 'AppDBContext.Visitings' пуст.");
            }
            if (_context.Diseases.Where(v => v.VisitingId == id).Count() != 0)
            {
                return Problem("Существую связанные данные (визиты).");
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

        public FileResult GetReport(String? dates)
        {
            // Путь к файлу с шаблоном
            List<DateTime> dateList = new List<DateTime>();
            if (dates != null)
            {

                foreach (string date in dates.Split("-"))
                {
                    dateList.Add(DateTime.Parse(date));
                }

            }
            var user = _userManager.GetUserAsync(User).Result;
            

            string path = "/Reports/templates/report_template_of_visitings.xlsx";
            //Путь к файлу с результатом
            string result = "/Reports/report_visitings.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Сеничкин Д.О.";
                excelPackage.Workbook.Properties.Title = "Список посещений";
                excelPackage.Workbook.Properties.Subject = "Посещения";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //плучаем лист по имени.
                ExcelWorksheet worksheet =
                    excelPackage.Workbook.Worksheets["Visitings"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Visiting> Visitings;
                if(dateList.Count > 0)
                {
                    Visitings = _context.Visitings.Include(v => v.Doctor).Include(v => v.Patient).Where(v => v.DateVisiting >= dateList[0] && v.DateVisiting <= dateList[1]).ToList();
                    worksheet.Cells[1,1].Value = worksheet.Cells[1,1].Value + " " + "[" + dateList[0].ToShortDateString() + " : " + dateList[1].ToShortDateString() + "]";
                }
                else
                {
                    Visitings = _context.Visitings.Include(v => v.Doctor).Include(v => v.Patient).ToList();
                }
                if (User.IsInRole("guest"))
                {
                    Visitings = Visitings.Where(v => v.PatientId == user.ModelId).ToList(); 
                }

                List<Doctor> Doctors = _context.Doctors.Include(d => d.Visitings).ToList();
                
                foreach (Doctor doctor in Doctors)
                {
                    bool rep = true;
                    if (doctor.Visitings != null)
                    {
                        foreach (Visiting visiting in Visitings)
                        {
                            if (doctor.Visitings.Contains(visiting))
                            {
                                worksheet.Cells[startLine, 1].Value = startLine - 2;
                                worksheet.Cells[startLine, 2].Value = visiting.DateVisiting;
                                worksheet.Cells[startLine, 3].Value = visiting.Patient.FirstName;
                                worksheet.Cells[startLine, 4].Value = visiting.Patient.LastName;
                                worksheet.Cells[startLine, 5].Value = visiting.Patient.Patronymic;
                                if(rep)
                                {
                                    worksheet.Cells[startLine, 6].Value = doctor.FirstName;
                                    worksheet.Cells[startLine, 7].Value = doctor.LastName;
                                    worksheet.Cells[startLine, 8].Value = doctor.Patronymic;
                                    rep = false;
                                }
                                startLine++;
                            }
                        }
                        
                    }
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
                // Тип файла - content-type
                string file_type =
                    "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
                // Имя файла - необязательно
                string file_name = "report_visitings.xlsx";
                return File(result, file_type, file_name);
            }
        }
    }
}
