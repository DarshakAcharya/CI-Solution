using CI_Platform.Models;
using CI_Platform.Models.ViewModels;
using CI_Platform_Entites.Data;
using CI_Platform_Entites.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CI_Platform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly CiPlatformContext _db;

        public HomeController(ILogger<HomeController> logger, CiPlatformContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(RegisterVM user)
        {
            if (ModelState.IsValid)
            {
                var obj = _db.Users.Where(e => e.Email == user.Email).FirstOrDefault();
                if (obj == null)
                {
                    // Hash the user's password using Bcrypt

                    //where(function(e){
                    //    return e.Email == user.Email
                    //})
                    var data = new User()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Password = user.Password,
                        PhoneNumber = user.PhoneNumber,
                        CountryId = null,
                        CityId = null
                    };
                    _db.Users.Add(data);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["userExists"] = "Email already exists";
                    return View(user);
                }


            }
            else
            {
                TempData["errorMessage"] = "Empty form can't be submitted";
                return View(user);
            }
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        public IActionResult LandingPage()
        {
            return View();
        }

        public IActionResult MissionListingPage()
        {
            return View();
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