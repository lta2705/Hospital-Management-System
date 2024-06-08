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
    public class IssuesController : Controller
    {
        private readonly HospitalDbContext _context = new();

        // GET: Issues
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
            var hospitalDbContext = _context.Issues.Include(i => i.Department);
            return View(await hospitalDbContext.ToListAsync());
        }

        // GET: Issues/Details/5
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

            var issue = await _context.Issues
                .Include(i => i.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (issue == null)
            {
                return NotFound();
            }

            return View(issue);
        }

        // GET: Issues/Create
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
            return View();
        }

        // POST: Issues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Name, string Description, double Fee, int DepartmentId)
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
            var find = _context.Issues.FirstOrDefault(i => i.Name == Name);
            var issue = new Issue
            {
                Name = Name,
                Description = Description,
                Fee = Fee,
                DepartmentId = DepartmentId
            };
            if(find != null)
            {
                ViewData["Message"] = "This issue already exist";
                return View(issue);
            }
            if (ModelState.IsValid)
            {
                _context.Add(issue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", issue.DepartmentId);
            return View(issue);
        }

        // GET: Issues/Edit/5
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

            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", issue.DepartmentId);
            return View(issue);
        }

        // POST: Issues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Name, string Description, double Fee, int DepartmentId)
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
            var issue = _context.Issues.FirstOrDefault(i => i.Id == id);
                
            if (issue == null)
            {
                ViewData["Message"] = "This issue is not exist";
                return View(issue);
            }
            else
            {
                issue.Name = Name;
                issue.Description = Description;
                issue.DepartmentId = DepartmentId;
                issue.Fee = Fee;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(issue).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IssueExists(issue.Id))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", issue.DepartmentId);
            return View(issue);
        }

        // GET: Issues/Delete/5
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

            var issue = await _context.Issues
                .Include(i => i.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (issue == null)
            {
                return NotFound();
            }

            return View(issue);
        }

        // POST: Issues/Delete/5
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
            var issue = await _context.Issues.FindAsync(id);
            if (issue != null)
            {
                _context.Issues.Remove(issue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IssueExists(int id)
        {
            return _context.Issues.Any(e => e.Id == id);
        }
    }
}
