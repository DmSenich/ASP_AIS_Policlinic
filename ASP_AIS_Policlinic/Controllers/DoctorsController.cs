﻿using System;
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
            
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = doctorId });
        }

        // GET: Doctors/Create
        public async Task<IActionResult> Create(bool? fromRecordDiagnosis)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
            {
                return new StatusCodeResult(403);
            }

            ViewBag.Specialties = _context.Specialties.ToList();
            if (fromRecordDiagnosis == true)
                ViewBag.toRecordDiagnosis = true;
            else
                ViewBag.toRecordDiagnosis = false;
            return View();
        }
        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doctor doctor, IFormFile upload, int[] selectedSpecialties)
        {
            if (ModelState.IsValid /*&& selectedSpecialties != null*/)
            {
                //foreach(var sp in _context.Specialties.Where(sp0 => selectedSpecialties.Contains(sp0.Id)))
                //{
                //    doctor.Specialties.Add(sp);
                //}
                if(upload != null)
                {
                    string path = "\\Files\\" + upload.FileName;
                    using(var fileStream = 
                        new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    doctor.PathPhoto = path;
                }

                _context.Add(doctor);
                await _context.SaveChangesAsync();

                if (doctor.toRecordDiagnosis == true)
                    return RedirectToAction("ChooseDoctor", "RecordDiagnosis");
                else
                    return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (!(await _userManager.IsInRoleAsync(user, "coach") || await _userManager.IsInRoleAsync(user, "admin")))
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
        public async Task<IActionResult> Edit(int id, Doctor doctor, int[] selectedSpecialties, IFormFile? upload)
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
                return Problem("Entity set 'AppDBContext.Doctors'  is null.");
            }
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
          return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
