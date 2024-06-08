using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hospital.Models;
using System.Numerics;

namespace Hospital.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly HospitalDbContext _context = new();

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var Role = HttpContext.Session.GetString("Role");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            var hospitalDbContext = _context.Appointments.Include(a => a.Doctor).Include(a => a.Issue).Include(u => u.User).Include(u => u.Doctor.User);
            if (Role=="Patient")
            {
                return View(await hospitalDbContext.Where(i=> i.UserId==UserID).ToListAsync());
            }
            if (Role == "Doctor")
            {
                return View(await hospitalDbContext.Where(i => i.DoctorId == UserID).ToListAsync());
            }
            return View(await hospitalDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Doctor.User)
                .Include(a => a.Issue)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors.Include(u => u.User).ToDictionary(u => u.Id, u => (u.User.Name + ' ' + u.Description)), "Key", "Value");
            ViewData["IssueId"] = new SelectList(_context.Issues.ToDictionary(u => u.Id, u => (u.Name + ": " + u.Description)), "Key", "Value");
            return View();
        }
        public ActionResult GetDoctor(int id)
        {
            var list = (from d in _context.Doctors
                        join i in _context.Issues
                        on d.DepartmentId equals i.DepartmentId
                        where i.Id == id
                        select new
                        {
                            d.Id,
                            d.User.Name,
                            d.User.Description,
                        }
                        ).ToList();
            var result = new List<Doctor>();
            foreach (var item in list)
            {
                result.Add(new Doctor
                {
                    Id = item.Id,
                    Description = item.Name + ": " + item.Description,
                });
            }
            return Json(new { data = result });
        }
        public ActionResult GetFee(int id)
        {
            var fee = _context.Issues.FirstOrDefault(i => i.Id == id).Fee;
            return Json(new { data = fee });
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Title, int DoctorId, int IssueId, DateTime Time, string Description)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var Name = HttpContext.Session.GetString("Name");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors.Include(u => u.User).ToDictionary(u => u.Id, u => (u.User.Name + ' ' + u.Description)), "Key", "Value");
            ViewData["IssueId"] = new SelectList(_context.Issues.ToDictionary(u => u.Id, u => (u.Name + ": " + u.Description)), "Key", "Value");
            var appointment = new Appointment
            {
                Title = Title,
                DoctorId = DoctorId,
                IssueId = IssueId,
                Time = Time,
                Description = Description,
                UserId = UserID
            };
            if (Time < DateTime.Now)
            {
                ViewData["Message"] = "Appointment cannot create in the past";
                return View(appointment);
            }
            var find = _context.Appointments.Where(u => u.Time == appointment.Time && u.DoctorId == appointment.DoctorId).FirstOrDefault();
            if (find != null)
            {
                ViewData["Message"] = "This doctor already have a appointment in " + appointment.Time;
                return View(appointment);
            }
            if (appointment.Time.TimeOfDay.CompareTo(new TimeSpan(9, 0, 0)) < 0 || appointment.Time.TimeOfDay.CompareTo(new TimeSpan(17, 0, 0)) > 1)
            {
                ViewData["Message"] = "This doctor only have a appointment between 9 AM to 5 PM";
                return View(appointment);
            }
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                var issue = _context.Issues.FirstOrDefault(i => i.Id == IssueId);
                var bill = new Bill
                {
                    IssueId = IssueId,
                    Title = "Appointment Bill from " + Name,
                    UserId = UserID,
                    TotalFee = issue.Fee
                };
                _context.Add(bill);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors.Include(u => u.User).ToDictionary(u => u.Id, u => (u.User.Name + ' ' + u.Description)), "Key", "Value");
            ViewData["IssueId"] = new SelectList(_context.Issues.ToDictionary(u => u.Id, u => (u.Name + ": " + u.Description)), "Key", "Value");
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Title, int DoctorId, int IssueId, DateTime Time, string Description)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors.Include(u => u.User).ToDictionary(u => u.Id, u => (u.User.Name + ' ' + u.Description)), "Key", "Value");
            ViewData["IssueId"] = new SelectList(_context.Issues.ToDictionary(u => u.Id, u => (u.Name + ": " + u.Description)), "Key", "Value");
            var appointment = _context.Appointments.FirstOrDefault(i => i.Id == id);
            if (appointment != null)
            {
                appointment.Title = Title;
                appointment.DoctorId = DoctorId;
                appointment.IssueId = IssueId;
                appointment.Time = Time;
                appointment.Description = Description;
            }
            else
            {
                ViewData["Message"] = "Appointment not exist";
                return View(appointment);
            }

            if (Time < DateTime.Now)
            {
                ViewData["Message"] = "Appointment cannot create in the past";
                return View(appointment);
            }
            var find = _context.Appointments.Where(u => u.Time == appointment.Time && u.DoctorId == appointment.DoctorId).FirstOrDefault();
            if (find != null)
            {
                ViewData["Message"] = "This doctor already have a appointment in " + appointment.Time;
                return View(appointment);
            }
            if (appointment.Time.TimeOfDay.CompareTo(new TimeSpan(9, 0, 0)) < 0 || appointment.Time.TimeOfDay.CompareTo(new TimeSpan(17, 0, 0)) > 1)
            {
                ViewData["Message"] = "This doctor only have a appointment between 9 AM to 5 PM";
                return View(appointment);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(appointment).State=EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
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
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor.User)
                .Include(a => a.Doctor)
                .Include(a => a.Issue)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
