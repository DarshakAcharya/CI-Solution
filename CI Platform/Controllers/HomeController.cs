using CI_Platform.Models;
using CI_Platform.Models.ViewModels;
using CI_Platform_Entites.Data;
using CI_Platform_Entites.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CI_Platform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly CiPlatformContext _db;

        private List<Mission> Missions = new List<Mission>();
        
        private List<MissionVM> missionVMList = new List<MissionVM>();

        public HomeController(ILogger<HomeController> logger, CiPlatformContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginVM model)

        {
            var Ab =_db.Users.FirstOrDefault(u => u.Email == model.Email && u.Password==model.Password);
            if (Ab == null)
            {
                ViewBag.loginerror = "email adress & password not match";
                return View();  
            }

            HttpContext.Session.SetString("UserId", Ab.UserId.ToString());

            return RedirectToAction("LandingPage", "Home" ,new{@id=Ab.UserId});
        }

        public IActionResult Filter()
        {
            
            return PartialView();
        }

         
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var ABC = _db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (ABC == null)
                {
                    ViewBag.Emailnotexist = "email address does not exist";
                }
            }
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

        public IActionResult LandingPage(long id)
        {
            string myVariable = HttpContext.Session.GetString("UserId");
            if(myVariable == null)
            {
                return RedirectToAction("Index");
            }
            var user = _db.Users.FirstOrDefault(e => e.UserId == id);
            ViewBag.user = user;
            var result = from Cmission in _db.Missions
                         join Mission_Theme in _db.MissionThemes on Cmission.ThemeId equals Mission_Theme.MissionThemeId
                         select new { Mission_Theme.Title,Cmission.MissionId};
            var MyCity = from Mission in _db.Missions
                         join City in _db.Cities on Mission.CityId equals City.CityId
                         select new { Mission.CityId, City.Name };
            var Goal = from Gmission in _db.Missions
                       join GoalMission in _db.GoalMissions on Gmission.MissionId equals GoalMission.MissionId
                       select new { Gmission.MissionId, GoalMission.GoalValue, GoalMission.GoalObjectiveText };
            Missions = _db.Missions.ToList();



            foreach (var mission in Missions)
            {
                var theme = result.Where(result => result.MissionId == mission.MissionId).FirstOrDefault();
                var city = MyCity.Where(MyCity => MyCity.CityId == mission.CityId).FirstOrDefault();
                var goal = Goal.Where(Goal => Goal.MissionId == mission.MissionId).FirstOrDefault();
                string[] startDate = mission.StartDate.ToString().Split(' ');
                string[] endDate = mission.EndDate.ToString().Split(' ');
                missionVMList.Add(new MissionVM()
                {
                    MissionId = mission.MissionId,
                    Title = mission.Title,
                    ShortDescription = mission.ShortDescription,
                    Description = mission.Description,
                    Organization = mission.OrganizationName,
                    OrganizationDetails = mission.OrganizationDetail,
                    //Rating = mission.MissionRatings,
                    //ADD MISSION IMAGE URL HERE
                    //Theme = mission.Theme,
                    //ADD PROGRESS HERE
                    //ADD recent volunteers here
                    missionType = mission.MissionType,
                    isFavrouite = mission.FavoriteMissions.Any(),
                    createdAt = DateTime.Now,
                    Theme = theme.Title,


                    StartDate = startDate[0],
                    EndDate = endDate[0],
                    //City = mission.City,
                    City = city.Name,
                    NoOfSeatsLeft =int.Parse(mission.Availability),
                    progress = int.Parse(goal.GoalValue),
                    GoalAim = goal.GoalObjectiveText,

                })  ;
            }

            MissionListingVM missionListingVM = new MissionListingVM
            {
                Missions = missionVMList,
                MissionCount = missionVMList.Count(),

            };

             
            return View(missionListingVM);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
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