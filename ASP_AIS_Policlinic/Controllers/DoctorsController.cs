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
using System.IO;

namespace ASP_AIS_Policlinic.Controllers
{
    [Authorize]
    public class DoctorsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<PoliclinicUser> _userManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public DoctorsController(AppDBContext context, UserManager<PoliclinicUser> userManaher, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManaher;
            _appEnvironment = appEnvironment;
        }

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.Doctors.Include(d => d.Specialties);
              return View(await appDBContext.ToListAsync());
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.Include(d=>d.Specialties)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }
            if (!String.IsNullOrEmpty(doctor.PathPhoto))
            {
                byte[] photodata = System.IO.File
                    .ReadAllBytes(_appEnvironment.WebRootPath + doctor.PathPhoto);
                ViewBag.Photodata = photodata;
            }
            else
            {
                ViewBag.Photodata = null;
            }

            DoctorDetailsViewModel viewModel = new DoctorDetailsViewModel();
            viewModel.Doctor = doctor;
            viewModel.Specialties = _context.Specialties.Where(sp => sp.Doctors.FirstOrDefault(d => d.Id == id).Id == id);
            return View(viewModel);
        }
        [Authorize(Roles = "coach, admin")]
        public async Task<IActionResult> AddSpecialtyToDoctor(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }
            ViewBag.DoctorId = id;
            return View(_context.Specialties.Include(sp => sp.Doctors));
        }

        [HttpPost]
        public async Task<IActionResult> AddSpecialtyToDoctor(int doctorId, int specialtyId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }
            Specialty specialty = _context.Specialties.Include(sp => sp.Doctors).FirstOrDefault(sp => sp.Id == specialtyId);
            Doctor doctor = _context.Doctors.Include(d => d.Specialties).FirstOrDefault(d => d.Id == doctorId);

#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            if (specialty.Doctors.Contains(doctor))
            {
                specialty.Doctors.Remove(doctor);
                doctor.Specialties.Remove(specialty);
            }
            else
            {
                specialty.Doctors.Add(doctor);
                doctor.Specialties.Add(specialty);
            }
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = doctorId });
        }

        // GET: Doctors/Create
        [Authorize(Roles = "coach, admin")]
        public async Task<IActionResult> Create(bool? fromRecordDiagnosis, bool? forProfile)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") && forProfile==true || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }

            ViewBag.Specialties = _context.Specialties.ToList();
            if (fromRecordDiagnosis == true)
                ViewBag.toRecordDiagnosis = true;
            else
                ViewBag.toRecordDiagnosis = false;

            if (forProfile == true)
                ViewBag.isProfile = true;
            else
                ViewBag.isProfile = false;
            return View();
        }
        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doctor doctor, IFormFile? upload)
        {
            if (ModelState.IsValid /*&& selectedSpecialties != null*/)
            {
                //foreach(var sp in _context.Specialties.Where(sp0 => selectedSpecialties.Contains(sp0.Id)))
                //{
                //    doctor.Specialties.Add(sp);
                //}
                string path;
                if (upload != null)
                {
                    path = "\\Files\\" + upload.FileName;
                    using (var fileStream =
                        new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    doctor.PathPhoto = path;

                }
                else if(System.IO.File.Exists(_appEnvironment.WebRootPath + "\\Files\\defaultPhoto.jpeg"))
                {
                    path = "\\Files\\defaultPhoto.jpeg";

                    doctor.PathPhoto = path;
                }

                _context.Add(doctor);
                await _context.SaveChangesAsync();

                if (doctor.toRecordDiagnosis == true)
                    return RedirectToAction("ChooseDoctor", "RecordDiagnosis");
                else if (doctor.toProfile == true)
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    user.ModelId = doctor.Id;
                    await _userManager.UpdateAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        [Authorize(Roles = "coach, admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") && user.ModelId == id || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            if (!String.IsNullOrEmpty(doctor.PathPhoto))
            {
                byte[] photodata = System.IO.File.
                    ReadAllBytes(_appEnvironment.WebRootPath + doctor.PathPhoto);
                ViewBag.Photodata = photodata;
            }
            else if (System.IO.File.Exists(_appEnvironment.WebRootPath + "\\Files\\defaultPhoto.jpeg"))
            {
                doctor.PathPhoto = "\\Files\\defaultPhoto.jpeg";
                byte[] photodata = System.IO.File.
                    ReadAllBytes(_appEnvironment.WebRootPath + doctor.PathPhoto);
                ViewBag.Photodata = photodata;
            }
            else
            {
                ViewBag.Photodata = null;
            }
            ViewBag.Specialties = _context.Specialties.ToList();
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Doctor doctor, IFormFile? upload)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid /*&& selectedSpecialties != null*/)
            {
                if(upload != null)
                {
                    string path = "\\Files\\" + upload.FileName;
                    using(var fileStream = 
                        new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    if (!String.IsNullOrEmpty(doctor.PathPhoto))
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath + doctor.PathPhoto);
                    }
                    doctor.PathPhoto = path;
                }

                try
                {

                    //Doctor newDoc = await _context.Doctors.FindAsync(id);
                    //newDoc.Specialties.Clear();
                    //foreach(var sp in _context.Specialties)
                    //{
                    //    if(selectedSpecialties.Contains(sp.Id))
                    //        newDoc.Specialties.Add(sp);
                    //}
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();

                    var user = await _userManager.GetUserAsync(User);
                    user.LastName = doctor.LastName;
                    user.FirstName = doctor.FirstName;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
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
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!await _userManager.IsInRoleAsync(user, "admin"))
            {
                return new StatusCodeResult(403);
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Doctors == null)
            {
                return Problem("Набор сущностей 'AppDBContext.Doctors' пуст.");
            }
            if (_context.Visitings.Where(v => v.DoctorId == id).Count() != 0)
            {
                return Problem("Существую связанные данные (визиты).");
            }
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                //doctor.Specialties.Clear();
                var specialties = _context.Specialties.Include(sp => sp.Doctors).Where(sp => sp.Doctors.Contains(doctor)).ToList();
                foreach (Specialty specialty in specialties)
                {
                    specialty.Doctors.Remove(doctor);
                }
                _context.Doctors.Remove(doctor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
          return _context.Doctors.Any(e => e.Id == id);
        }

        public FileResult GetReport()
        {
            // Путь к файлу с шаблоном
            string path = "/Reports/templates/report_template_of_specialtyDoctors_ALL.xlsx";
            //Путь к файлу с результатом
            string result = "/Reports/report_doctors.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Сеничкин Д.О.";
                excelPackage.Workbook.Properties.Title = "Список врачей";
                excelPackage.Workbook.Properties.Subject = "Врачи";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //плучаем лист по имени.
                ExcelWorksheet worksheet =
                    excelPackage.Workbook.Worksheets["Doctors"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Doctor> Doctors = _context.Doctors.Include(d => d.Specialties).ToList();
                foreach(Doctor doctor in Doctors)
                {
                    List<Specialty> specialties = doctor.Specialties.ToList();
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = doctor.LastName;
                    worksheet.Cells[startLine, 3].Value = doctor.FirstName;
                    worksheet.Cells[startLine, 4].Value = doctor.Patronymic;
                    worksheet.Cells[startLine, 5].Value = doctor.WorkExperience;
                    if(specialties != null)
                    {
                        string[] strSp = new string[specialties.Count];
                        int i = 0;
                        foreach(Specialty specialty in specialties)
                        {
                            strSp[i] = specialty.NameSpecialty;
                            i++;
                        }
                        worksheet.Cells[startLine, 6].Value = String.Join(", ", strSp);
                    }
                    startLine++;
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
                // Тип файла - content-type
                string file_type =
                    "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
                // Имя файла - необязательно
                string file_name = "report_doctors.xlsx";
                return File(result, file_type, file_name);
            }
        }
    }
}
