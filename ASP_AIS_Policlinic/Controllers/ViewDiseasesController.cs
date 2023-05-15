using ASP_AIS_Policlinic.Areas.Identity.Data;
using ASP_AIS_Policlinic.Models;
using ASP_AIS_Policlinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ASP_AIS_Policlinic.Controllers
{
    [Authorize]
    public class ViewDiseasesController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<PoliclinicUser> _userManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public ViewDiseasesController(AppDBContext context, UserManager<PoliclinicUser> userManager, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
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
            ViewBag.PatientId = id;
            var diseases = await _context.Diseases.Include(d => d.DiseaseType).Include(d => d.Visiting).Where(d=>d.Visiting.PatientId == id).ToListAsync();
            
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

        public FileResult? GetReport(int? id) //id patient
        {
            if (id == null || _context.Patients == null)
            {
                return null;
            }
            // Путь к файлу с шаблоном
            string path = "/Reports/templates/report_template_of_historyDiseases.xlsx";
            //Путь к файлу с результатом
            string result = "/Reports/report_historyDiseases.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Сеничкин Д.О.";
                excelPackage.Workbook.Properties.Title = "История болезни";
                excelPackage.Workbook.Properties.Subject = "Заболевания";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //плучаем лист по имени.
                ExcelWorksheet worksheet =
                    excelPackage.Workbook.Worksheets["Diseases"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Disease> Diseases = _context.Diseases.Include(d => d.DiseaseType).ToList();
                List<Visiting> Visitings = _context.Visitings.Include(v => v.Doctor).Include(v => v.Patient).Include(v => v.Diseases).Where(v => v.PatientId == id).ToList();

                //worksheet.Cells[startLine, 10].Value = _context.DiseaseTypes.Where(dt => dt.Id == id).First().NameDisease;
                worksheet.Cells[startLine, 9].Value = _context.Patients.Where(p => p.Id == id).First().LastName;
                worksheet.Cells[startLine, 10].Value = _context.Patients.Where(p => p.Id == id).First().FirstName;
                worksheet.Cells[startLine, 11].Value = _context.Patients.Where(p => p.Id == id).First().Patronymic;
                foreach (Visiting visiting in Visitings)
                {
                    foreach (Disease disease in Diseases)
                    {
                        if (visiting.Diseases.Contains(disease))
                        {
                            worksheet.Cells[startLine, 1].Value = startLine - 2;
                            worksheet.Cells[startLine, 2].Value = disease.Id;
                            worksheet.Cells[startLine, 3].Value = visiting.DateVisiting;

                            worksheet.Cells[startLine, 4].Value = visiting.Doctor.LastName;
                            worksheet.Cells[startLine, 5].Value = visiting.Doctor.FirstName;
                            worksheet.Cells[startLine, 6].Value = visiting.Doctor.Patronymic;
                            worksheet.Cells[startLine, 7].Value = disease.DiseaseType.NameDisease;
                            worksheet.Cells[startLine, 8].Value = disease.Description;
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
                string file_name = "report_historyDisease.xlsx";
                return File(result, file_type, file_name);
            }
        }
    }
}
