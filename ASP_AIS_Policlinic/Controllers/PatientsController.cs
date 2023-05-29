﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_AIS_Policlinic.Models;
using Microsoft.AspNetCore.Authorization;
using ASP_AIS_Policlinic.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Numerics;

namespace ASP_AIS_Policlinic.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<PoliclinicUser> _userManager;

        public PatientsController(AppDBContext context, UserManager<PoliclinicUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
              return View(await _context.Patients.ToListAsync());
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if(User.IsInRole("guest") && id != user.ModelId)
            {
                return new StatusCodeResult(403);
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create(bool? fromRecordDiagnosis, bool? forProfile)
        {
                if (fromRecordDiagnosis == true)
                    ViewBag.toRecordDiagnosis = true;
                else
                    ViewBag.toRecordDiagnosis = false;
                if(forProfile == true)
                    ViewBag.isProfile = true;
                else
                    ViewBag.isProfile = false;
                return View();      
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                if (patient.toRecordDiagnosis == true)
                    return RedirectToAction("Index", "RecordDiagnosis");
                else if (patient.toProfile == true)
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    user.ModelId = patient.Id;
                    await _userManager.UpdateAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction(nameof(Index));

            }
            return View(patient);
        }

        // GET: Patients/Edit/5
        [Authorize(Roles = "guest, admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LastName,FirstName,Patronymic,Area,City,House,Apartment,DateBirth")] Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();

                    var user = await _userManager.GetUserAsync(User);
                    user.LastName = patient.LastName;
                    user.FirstName = patient.FirstName;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
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
            return View(patient);
        }

        // GET: Patients/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Patients == null)
            {
                return Problem("Набор сущностей  'AppDBContext.Patients'  пуст.");
            }
            if (_context.Visitings.Where(v => v.PatientId == id).Count() != 0)
            {
                return Problem("Существую связанные данные (визиты).");
            }
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
          return _context.Patients.Any(e => e.Id == id);
        }
    }
}
