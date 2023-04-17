 

using CI_Platform.Models;
using CI_Platform.Models.ViewModels;
using CI_Platform_Entites.Data;
using CI_Platform_Entites.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                ViewBag.loginerror = "Email address & password does not match";
                return View();  
            }

            HttpContext.Session.SetString("UserId", Ab.UserId.ToString());

            return RedirectToAction("LandingPage", "Home" ,new{@id=Ab.UserId});
        }

        [HttpPost]
        public async Task<IActionResult> FilterMissions(string? SearchInput, long[] CountryFilter, long[] CityFilter, long[] MissionThemeFilter, long[] MissionSkillFilter, string MissionSort = "", int pg = 1)
        {
            const int pageSize = 3;
            Missions = _db.Missions.Include(m => m.MissionSkills).ToList();

            long UserId = Convert.ToInt64(HttpContext.Session.GetString("UserId"));
            var user = _db.Users.FirstOrDefault(e => e.UserId == UserId);
            ViewBag.user = user;

            var filter = new FilterVM
            {
                SearchInput = SearchInput?.ToLower(),
                CountryFilter = CountryFilter,
                CityFilter = CityFilter,
                ThemesFilter = MissionThemeFilter,
                SkillsFilter = MissionSkillFilter,

            };

            //Pagination Code
        //        int missionCounts = Missions.Count();
        //    if (pg < 1)
        //        pg = 1;
        //    int totalPages = Pager.getTotalPages(missionCounts, pageSize);
        //    if (pg > totalPages)
        //        pg = totalPages;
        //    int recSkip = (pg - 1) * pageSize;
        //    var pager = new Pager(missionCounts, pg, pageSize);
        //    // Missions on Current page
        //    List<MissionsDetailsVM> PageMissionsDetails = MissionsDetails.Skip(recSkip).Take(pager.PageSize).ToList();

        //    MissionListingVM MissionListing = new MissionListingVM
        //    {
        //        MissionCount = missionCounts,                                                     
        //        Pager = pager,
        //        Missions = PageMissionsDetails
        //    };

        //    return PartialView("Index", MissionListing);
        //}




        var result = from Cmission in _db.Missions
                         join Mission_Theme in _db.MissionThemes on Cmission.ThemeId equals Mission_Theme.MissionThemeId
                         select new { Mission_Theme.Title, Cmission.MissionId };
            var MyCity = from Mission in _db.Missions
                         join City in _db.Cities on Mission.CityId equals City.CityId
                         select new { Mission.CityId, City.Name };
            var Goal = from Gmission in _db.Missions
                       join GoalMission in _db.GoalMissions on Gmission.MissionId equals GoalMission.MissionId
                       select new { Gmission.MissionId, GoalMission.GoalValue, GoalMission.GoalObjectiveText };
            Missions = _db.Missions.Include(m => m.MissionSkills).ToList();
            ViewBag.Countries = _db.Countries.ToList();
            ViewBag.Cities = _db.Cities.ToList();
            ViewBag.Themes = _db.MissionThemes.ToList();
            ViewBag.Skills = _db.Skills.ToList();

            var Skills = _db.Skills.Where(m => m.DeletedAt == null);


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
                    CityId = mission.CityId,
                    CountryId = mission.CountryId,
                    ThemeId = mission.ThemeId,


                    StartDate = startDate[0],
                    EndDate = endDate[0],
                    //City = mission.City,
                    City = city.Name,
                    NoOfSeatsLeft = int.Parse(mission.Availability),
                    progress = int.Parse(goal.GoalValue),
                    GoalAim = goal.GoalObjectiveText,

                    MissionSkills = mission.MissionSkills.Join(Skills, ms => ms.MissionSkillId, s => s.SkillId, (ms, s) => ms).ToList(),


                });
            }

            // filtering
            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.SearchInput))
                {
                    missionVMList = missionVMList.Where(m => m.Title.ToLower().Contains(filter.SearchInput)).ToList();
                }
                if (filter.CountryFilter != null && filter.CountryFilter.Length > 0)
                {
                    //foreach (var country in filter.CountryFilter)
                    //{
                        missionVMList = missionVMList.Where(m => filter.CountryFilter.Any(x => x == m.CountryId)).ToList();
                    //}
                }
                if (filter.CityFilter != null && filter.CityFilter.Length > 0)
                {
                    missionVMList = missionVMList.Where(m => filter.CityFilter.Any(cf => cf == m.CityId)).ToList();
                }
                if (filter.ThemesFilter != null && filter.ThemesFilter.Length > 0)
                {
                    missionVMList = missionVMList.Where(m => filter.ThemesFilter.Any(tf => tf == m.ThemeId)).ToList();
                }
                if (filter.SkillsFilter != null && filter.SkillsFilter.Length > 0)
                {
                    missionVMList = missionVMList.Where(m => m.MissionSkills.Any(ms => filter.SkillsFilter.Any(sf => sf == ms.SkillId))).ToList();
                }
            }


            MissionListingVM missionListingVM = new MissionListingVM();
            missionListingVM.Missions = missionVMList;
            missionListingVM.MissionCount = missionVMList.Count();
            ViewBag.missionListingVM = missionListingVM;


            return PartialView("_MissionListingPartial", missionListingVM);
        }

        //public async Task<IActionResult> FilterMissions(string SearchInput, string[] CountryFilter, string[] CityFilter, string[] MissionThemeFilter, string[] MissionSkillFilter, string MissionSort = "", int pg = 1)
        //{
        //    string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    const int pageSize = 3;
        //    var filter = new MissionFilter
        //    {
        //        SearchInput = SearchInput,
        //        Country = CountryFilter,
        //        City = CityFilter,
        //        MissionThemes = MissionThemeFilter,
        //        MissionSkills = MissionSkillFilter
        //    };
        //    List<MissionsDetailsVM> MissionsDetails = _unitOfWork.Mission.GetAllMissions(userId, filter, MissionSort);
        //    // Pagination Code
        //    int missionCounts = MissionsDetails.Count();
        //    if (pg < 1)
        //        pg = 1;
        //    int totalPages = Pager.getTotalPages(missionCounts, pageSize);
        //    if (pg > totalPages)
        //        pg = totalPages;
        //    int recSkip = (pg - 1) * pageSize;
        //    var pager = new Pager(missionCounts, pg, pageSize);
        //    // Missions on Current page
        //    List<MissionsDetailsVM> PageMissionsDetails = MissionsDetails.Skip(recSkip).Take(pager.PageSize).ToList();

        //    MissionListingVM MissionListing = new MissionListingVM
        //    {
        //        MissionCount = missionCounts,
        //        Pager = pager,
        //        Missions = PageMissionsDetails
        //    };

        //    return PartialView("Index", MissionListing);
        //}


        
         


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
                    ViewBag.Emailnotexist = "Email address does not exist";
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
            Missions = _db.Missions.Include(m => m.MissionSkills).ToList();
            ViewBag.Countries = _db.Countries.ToList();
            ViewBag.Cities = _db.Cities.ToList();
            ViewBag.Themes = _db.MissionThemes.ToList();
            ViewBag.Skills = _db.Skills.ToList();

            var Skills = _db.Skills.Where(m => m.DeletedAt == null);


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

                    MissionSkills = mission.MissionSkills.Join(Skills, ms => ms.MissionSkillId, s => s.SkillId, (ms, s) => ms).ToList(),


                })  ;
            }

            MissionListingVM missionListingVM = new MissionListingVM
            {
                Missions = missionVMList,
                MissionCount = missionVMList.Count(),

            };

             
            return View(missionListingVM);
        }

        public IActionResult MissionDetailPage(long id, long missionId )
        {
            var user = _db.Users.FirstOrDefault(e => e.UserId == id);
            var Mission = _db.Missions.FirstOrDefault(m => m.MissionId == missionId);
            var GoalMission = _db.GoalMissions.FirstOrDefault( g => g.MissionId == missionId);
            //var city = _db.Cities.FirstOrDefault(c => c.CityId == Mission.CityId);
            var theme = _db.MissionThemes.FirstOrDefault(c => c.MissionThemeId == Mission.ThemeId);
            var StartDate = Mission.StartDate.ToString().Split(' ');
            var EndDate = Mission.EndDate.ToString().Split(' ');
            var skills = _db.MissionSkills.Include(s => s.Skill).ToList();
            
             
            ViewBag.user = user;
            ViewBag.Mission = Mission;
            ViewBag.GoalMission = GoalMission;
            //ViewBag.City = city;
            ViewBag.Theme = theme;
            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            ViewBag.Skills = skills;
            
            
            return View();
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

        
        public IActionResult StoryListingPage(long id)
        {
            var user = _db.Users.FirstOrDefault(e => e.UserId == id);
            ViewBag.Countries = _db.Countries.ToList();
            ViewBag.Cities = _db.Cities.ToList();
            ViewBag.Themes = _db.MissionThemes.ToList();
            ViewBag.Skills = _db.Skills.ToList();

            ViewBag.user = user;
            return View();
        }

        public IActionResult ShareYourStoryPage(long id)
        {
            var user = _db.Users.FirstOrDefault(e => e.UserId == id);
            ViewBag.Countries = _db.Countries.ToList();
            ViewBag.Cities = _db.Cities.ToList();
            ViewBag.Themes = _db.MissionThemes.ToList();
            ViewBag.Skills = _db.Skills.ToList();

            ViewBag.user = user;

            return View();
        }

        public IActionResult StoryDetailPage(long id)
        {
            
            var user = _db.Users.FirstOrDefault(e => e.UserId == id);
            ViewBag.Countries = _db.Countries.ToList();
            ViewBag.Cities = _db.Cities.ToList();
            ViewBag.Themes = _db.MissionThemes.ToList();
            ViewBag.Skills = _db.Skills.ToList();

            ViewBag.user = user;

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