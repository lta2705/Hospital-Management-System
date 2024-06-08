using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hospital.Models;

namespace Hospital.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly HospitalDbContext _context = new();

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            if (role == "Patient")
            {
                return RedirectToAction("Index", "Home");
            }
            var hospitalDbContext = _context.Doctors.Include(d => d.Department).Include(d => d.User);
            return View(await hospitalDbContext.ToListAsync());
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            if (role == "Patient")
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            if (role == "Patient")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users.Where(u => u.Role == "Doctor"), "Id", "Name");
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int UserId, int DepartmentId, string Degree, int Salary, string Description)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            if (role == "Patient")
            {
                return RedirectToAction("Index", "Home");
            }
            var find = _context.Doctors.FirstOrDefault(u => u.UserId == UserId);
            var doctor = new Doctor
            {
                UserId = UserId,
                DepartmentId = DepartmentId,
                Degree = Degree,
                Salary = Salary,
                Description = Description
            };
            if (ModelState.IsValid && find == null)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                var user = _context.Users.FirstOrDefault(u => u.Id == UserId);
                user.Role = "Doctor";
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", doctor.DepartmentId);
            ViewData["UserId"] = new SelectList(_context.Users.Where(u => u.Role == null), "Id", "Name", doctor.UserId);
            ViewData["Message"] = "This user already is a doctor";
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            if (role == "Patient")
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", doctor.DepartmentId);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, int DepartmentId, string Degree, int Salary, string Description)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            if (role == "Patient")
            {
                return RedirectToAction("Index", "Home");
            }
            var doctor = _context.Doctors.FirstOrDefault(u => u.Id == Id);
            if(doctor !=null)
            {
                doctor.DepartmentId = DepartmentId;
                doctor.Degree = Degree;
                doctor.Salary = Salary;
                doctor.Description = Description;
            }
            else
            {
                ViewData["Message"] = "Doctor not exist";
                return View(doctor);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(doctor).State= EntityState.Modified;
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", doctor.DepartmentId);
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            if (role == "Patient")
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .Include(d => d.User)
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
            var UserID = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            if (role == "Patient")
            {
                return RedirectToAction("Index", "Home");
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
