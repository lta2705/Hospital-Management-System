using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections;

namespace Hospital.Controllers
{
    public class AccountsController : Controller
    {
        private readonly HospitalDbContext _context = new();

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            if(role == "Patient")
            {
                return RedirectToAction("Index", "Home");
            }
            var hospitalDbContext = _context.Accounts.Include(a => a.User);
            return View(await hospitalDbContext.ToListAsync());
        }

        // GET: Accounts/Details/5
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

            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Username, string Password, bool IsActive)
        {
            var account = _context.Accounts.FirstOrDefault(i => i.Id == id);
            if (account == null)
            {
                ViewData["Message"] = "Account not found";
                return View();
            }
            account.Username = Username;
            account.Password = Password;
            account.IsActive = IsActive;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(account).State=EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
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
            return View(account);
        }

        // GET: Accounts/Delete/5
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

            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
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
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                account.IsActive = false;
                _context.Accounts.Entry(account).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
