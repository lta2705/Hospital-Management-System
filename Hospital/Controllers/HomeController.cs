using Hospital.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Hospital.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HospitalDbContext _context = new();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID == null || UserID == -1)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID != null && UserID != -1)
            {
                return RedirectToAction("Index", "Home");
            }
            var Session = HttpContext.Session;
            try
            {
                if (Username != null && Password != null)
                {
                    var account = _context.Accounts.Where(u => u.Username == Username && u.Password == Password && u.IsActive == true).Include(u => u.User).ToList();
                    if (account.Count == 1)
                    {
                        Session.SetString("Name", account[0].User.Name);
                        Session.SetInt32("UserID", account[0].User.Id);
                        Session.SetString("Role", account[0].User.Role);
                        Session.SetInt32("AccountID", account[0].Id);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Session.SetInt32("UserID", -1);
                        ViewBag.message = "User Name or Password is incorrect!";
                    }
                }
                else
                {
                    Session.SetInt32("UserID", -1);
                    ViewBag.message = "Some unexpected issue is occure please try again!";
                }
            }
            catch (Exception ex)
            {
                Session.SetInt32("UserID", -1);
                ViewBag.message = "Some unexpected issue is occure please try again!";
            }
            return View("Login");
        }
        public IActionResult Register()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID != null && UserID != -1)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Register(string Name, string Email, string PhoneNumber, string Sex, string Address, string Password)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID != null && UserID != -1)
            {
                return RedirectToAction("Index", "Home");
            }
            var find = _context.Users.FirstOrDefault(u => u.Email == Email);
            var user = new User
            {
                Email = Email,
                PhoneNumber = PhoneNumber,
                Sex = Sex,
                Address = Address,
                Name = Name,
                Role = "Patient"
            };
            if (find != null)
            {
                ViewData["Message"] = "Email existed";
                return View(user);
            }
            if (ModelState.IsValid)
            {
                _context.Add(user);
                _context.SaveChanges();
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
                return Login(Email, Password);
            }
            ViewData["Message"] = "Fail to create user";
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.SetInt32("UserID", -1);
            return RedirectToAction("Login", "Home");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
