using CI_Platform.Models.ViewModels;
using CI_Platform_Entites.Data;
using CI_Platform_Entites.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace CI_Platform.Controllers
{
    public class UserMailController : Controller
    {
        private readonly CiPlatformContext _db;
        private readonly ILogger<HomeController> _logger;
        public UserMailController(ILogger<HomeController> logger, CiPlatformContext db)
        {
            _logger = logger;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordVM(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    return RedirectToAction("Forget", "user");
                    //ViewBag.Forget = "Enter Valid Email";
                }

                var token = Guid.NewGuid().ToString();

                var passwordReset = new PasswordReset
                {
                    Email = model.Email,
                    Token = token
                };

                _db.PasswordResets.Add(passwordReset);
                _db.SaveChanges();

                var resetLink = Url.Action("ResetPassword", "UserMail", new { email = model.Email, token }, Request.Scheme);

                var fromAddress = new MailAddress("systromysolutions@gmail.com", "CI Platform");
                var toAddress = new MailAddress(model.Email);
                var subject = "Password reset request";
                var body = $"Hi,<br /><br />Please click on the following link to reset your password:<br /><br /><a href='{resetLink}'>{resetLink}</a>";

                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    //Credentials = new networkcredential("tatvahl@gmail.com", "dvbexvljnrhcflfw"),
                    Credentials = new NetworkCredential("systromysolutions@gmail.com", "mjoagjrqrlmgodwl"),
                    EnableSsl = true
                };
                smtpClient.Send(message);
                

                return RedirectToAction("Index", "Home");
            }

            return View();

        }
 
            public IActionResult ResetPassword(string email, string token)
            {
            var passwordReset = _db.PasswordResets.FirstOrDefault(u => u.Email == email && u.Token == token);
                if (passwordReset == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                // Pass the email and token to the view for resetting the password\
                ViewBag.Email = email;
            ViewBag.Token = token;  
                return View();
            }

            


            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public IActionResult ResetPassword(ResetPasswordVM model)
            {
                if (ModelState.IsValid)
                {
                    // Find the user by email
                    var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);
                    if (user == null)
                    {
                        return RedirectToAction("ForgotPassword", "Home");
                    }

                    // Find the password reset record by email and token
                    var passwordReset = _db.PasswordResets.FirstOrDefault(u => u.Email == model.Email && u.Token == model.Token);
                    if (passwordReset == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    // Update the user's password
                    user.Password = model.Password;
                    _db.SaveChanges();

                }

                return RedirectToAction("Index", "Home");
            }

        }
    }

