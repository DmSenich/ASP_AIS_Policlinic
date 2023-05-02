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
using OfficeOpenXml;
using System.Numerics;

namespace ASP_AIS_Policlinic.Controllers
{
    [Authorize(Roles = "coach, admin")]
    public class DiseaseTypesController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<PoliclinicUser> _userManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public DiseaseTypesController(AppDBContext context, UserManager<PoliclinicUser> userManaher, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManaher;
            _appEnvironment = appEnvironment;
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

        public FileResult? GetReport(int? id)
        {
            if (id == null || _context.DiseaseTypes == null)
            {
                return null;
            }
            // Путь к файлу с шаблоном
            string path = "/Reports/templates/report_template_of_diseasesType.xlsx";
            //Путь к файлу с результатом
            string result = "/Reports/report_diseasesType.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Сеничкин Д.О.";
                excelPackage.Workbook.Properties.Title = "Список заболеваний по диагнозу";
                excelPackage.Workbook.Properties.Subject = "Заболевания";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //плучаем лист по имени.
                ExcelWorksheet worksheet =
                    excelPackage.Workbook.Worksheets["Diseases"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Disease> Diseases = _context.Diseases.Include(d => d.DiseaseType).Where(d => d.DiseaseTypeId == id).ToList();
                List<Visiting> Visitings = _context.Visitings.Include(v => v.Doctor).Include(v => v.Patient).Include(v => v.Diseases).ToList();
                
                worksheet.Cells[startLine, 10].Value = _context.DiseaseTypes.Where(dt => dt.Id == id).First().NameDisease;
                foreach (Visiting visiting in Visitings)
                {
                    foreach (Disease disease in Diseases)
                    {
                        if (visiting.Diseases.Contains(disease))
                        {
                            worksheet.Cells[startLine, 1].Value = startLine - 2;
                            worksheet.Cells[startLine, 2].Value = disease.Id;
                            worksheet.Cells[startLine, 3].Value = visiting.DateVisiting;
                            worksheet.Cells[startLine, 4].Value = visiting.Patient.FirstName;
                            worksheet.Cells[startLine, 5].Value = visiting.Patient.LastName;
                            worksheet.Cells[startLine, 6].Value = visiting.Patient.Patronymic;
                            worksheet.Cells[startLine, 7].Value = visiting.Doctor.FirstName;
                            worksheet.Cells[startLine, 8].Value = visiting.Doctor.LastName;
                            worksheet.Cells[startLine, 9].Value = visiting.Doctor.Patronymic;
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
                string file_name = "report_diseasesType.xlsx";
                return File(result, file_type, file_name);
            }
        }

        public FileResult? GetReportAll()
        {
            if (_context.DiseaseTypes == null)
            {
                return null;
            }
            // Путь к файлу с шаблоном
            string path = "/Reports/templates/report_template_of_diseasesType.xlsx";
            //Путь к файлу с результатом
            string result = "/Reports/report_diseasesTypeAll.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Сеничкин Д.О.";
                excelPackage.Workbook.Properties.Title = "Список заболеваний по диагнозу";
                excelPackage.Workbook.Properties.Subject = "Заболевания";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //плучаем лист по имени.
                ExcelWorksheet worksheet =
                    excelPackage.Workbook.Worksheets["Diseases"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<DiseaseType> DiseaseTypes = _context.DiseaseTypes.Include(dt => dt.Diseases).ToList();
                List<Disease> Diseases = _context.Diseases.Include(d => d.DiseaseType).ToList();
                List<Visiting> Visitings = _context.Visitings.Include(v => v.Doctor).Include(v => v.Patient).Include(v => v.Diseases).ToList();

                

                foreach(DiseaseType diseaseType in DiseaseTypes) 
                {
                    if(diseaseType.Diseases != null)
                    {
                        worksheet.Cells[startLine, 10].Value = diseaseType.NameDisease;
                        foreach (Visiting visiting in Visitings)
                        {
                            foreach (Disease disease in Diseases)
                            {
                                if (visiting.Diseases.Contains(disease))
                                {
                                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                                    worksheet.Cells[startLine, 2].Value = disease.Id;
                                    worksheet.Cells[startLine, 3].Value = visiting.DateVisiting;
                                    worksheet.Cells[startLine, 4].Value = visiting.Patient.FirstName;
                                    worksheet.Cells[startLine, 5].Value = visiting.Patient.LastName;
                                    worksheet.Cells[startLine, 6].Value = visiting.Patient.Patronymic;
                                    worksheet.Cells[startLine, 7].Value = visiting.Doctor.FirstName;
                                    worksheet.Cells[startLine, 8].Value = visiting.Doctor.LastName;
                                    worksheet.Cells[startLine, 9].Value = visiting.Doctor.Patronymic;
                                    startLine++;
                                }
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
                string file_name = "report_diseasesTypeAll.xlsx";
                return File(result, file_type, file_name);
            }
        }
    }
}
