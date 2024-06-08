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
    public class UsersController : Controller
    {
        private readonly HospitalDbContext _context = new();

        // GET: Users
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
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
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

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
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
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Name, string Email,string Password, string PhoneNumber, string Sex, string Address, string Role, string Description)
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
            var find = _context.Users.FirstOrDefault(u => u.Email == Email);
            var user = new User
            {
                Email = Email,
                Password = Password,
                PhoneNumber = PhoneNumber,
                Sex = Sex,
                Address = Address,
                Role = Role,
                Description = Description,
                Name = Name
            };
            if (find != null)
            {
                ViewData["Message"] = "Email existed";
                return View(user);
            }
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                var newUser = _context.Users.FirstOrDefault(u => u.Email == Email);
                var account = new Account
                {
                    Username = Email,
                    Password = Password,
                    IsActive = true,
                    UserId = newUser.Id
                };
                _context.Accounts.Add(account);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
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

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Name, string Email, string Role, string PhoneNumber, string Sex, string Address, string Description)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            var find = _context.Users.FirstOrDefault(u => u.Id == id);
            if (find == null)
            {
                ViewData["Message"] = "User not exist";
                return View(find);
            }
            else
            {
                find.Name = Name;
                find.Email = Email;
                find.Password = find.Password;
                find.PhoneNumber = PhoneNumber;
                find.Sex = Sex;
                find.Address = Address;
                find.Description = Description;
                find.Role = Role;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(find).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(find.Id))
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
            return View(find);
        }

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

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                if (user.Role == "Doctor")
                {
                    var doctor = _context.Doctors.FirstOrDefault(u => u.UserId == id);
                    if (doctor != null) _context.Remove(doctor);
                    var appointmentList = _context.Appointments.Where(u => u.DoctorId == doctor.Id);
                    if (appointmentList != null)
                    {
                        foreach (var appointment in appointmentList)
                        {
                            _context.Appointments.Remove(appointment);
                        }
                    }
                }
                if (user.Role == "Patient")
                {
                    var billList = _context.Bills.Where(u => u.UserId == id);
                    if (billList != null)
                    {
                        foreach (var bill in billList)
                        {
                            _context.Bills.Remove(bill);
                        }
                    }
                    var appointmentList = _context.Appointments.Where(u => u.UserId == id);
                    if (appointmentList != null)
                    {
                        foreach (var appointment in appointmentList)
                        {
                            _context.Appointments.Remove(appointment);
                        }
                    }
                }
                _context.Users.Remove(user);
                var acc = _context.Accounts.FirstOrDefault(u => u.UserId == id);
                if (acc != null) _context.Remove(acc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}