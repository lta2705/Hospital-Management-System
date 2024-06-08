using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hospital.Models;
using System.Data;

namespace Hospital.Controllers
{
    public class BillsController : Controller
    {
        private readonly HospitalDbContext _context = new();

        // GET: Bills
        public async Task<IActionResult> Index()
        {
            var Role = HttpContext.Session.GetString("Role");
            var UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            var hospitalDbContext = _context.Bills.Include(b => b.Issue).Include(b => b.User);
            if (Role == "Patient")
            {
                return View(await hospitalDbContext.Where(i => i.UserId == UserID).ToListAsync());
            }
            return View(await hospitalDbContext.ToListAsync());
        }

        // GET: Bills/Details/5
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

            var bill = await _context.Bills
                .Include(b => b.Issue)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }
    }
}
