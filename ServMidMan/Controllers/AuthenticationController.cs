using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServMidMan.Data;
using ServMidMan.Helper;
using ServMidMan.Models;
using ServMidMan.Services;
using CaptchaMvc.HtmlHelpers;

namespace ServMidMan.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly DataProviderContext _dataProvider;
        private string sessionVerificationCode;
        public AuthenticationController(DataProviderContext dataContext)
        {
            _dataProvider = dataContext;
            //bool canConnect = _dataProvider.Database.CanConnect();
            //_dataProvider.Users.Add(new User()
            //{
            //    Email = "test",
            //    Password = "test",
            //    EmailConfirmed = "test",
            //    Name = "test",

            //});
            //_dataProvider.SaveChanges();

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Welcome()
        {
            //ExcelDataTransmitter excelDataTransmitter = new ExcelDataTransmitter(_dataProvider);
            HttpContext.Session.SetString("Login","False");
			if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            }

            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
			return View();
        }
        public IActionResult Register(UserWithRegister user)
        {
            return View();
        }
        [HttpPost]
        public IActionResult Captcha(string check)
        {
            return View(check);
        }
        [HttpPost]
        public IActionResult RegisterHandling(UserWithRegister user, string verificationCode)
        {
            try
            {
                var isThereAnyEqualEmails = _dataProvider.Users.Where(x => x.Email == user.Email).FirstOrDefault();
                if(isThereAnyEqualEmails is not null)
                {
                    ViewBag.ErrorMessage = "Foglalt email";
                    return View("Register");
                }
                if(EmailVerificator.emailWithVerification.ContainsKey(user.Email))
                {
                    (string,DateTime) myValidationCodeWithDate;
                    EmailVerificator.emailWithVerification.TryGetValue(user.Email, out myValidationCodeWithDate);
                    if(myValidationCodeWithDate.Item1 != verificationCode) 
                    {
                        ViewBag.ErrorMessage = "Helytelen ellenőrző kód";
                        return View("Register");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Helytelen Email";
                    return View("Register");
                }
                user.Password = PasswordHasher.HashPassword(user.Password);
                user.LastLogin = DateTime.Now;
                _dataProvider.Users.Add(user as User);
                _dataProvider.SaveChanges();

                return View("Welcome");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Foglalt email";
            }
            return RedirectToAction("Register");
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            var pw = PasswordHasher.HashPassword(user.Password);
            var resultUser = _dataProvider.Users.Where(x=>x.Email == user.Email && x.Password == pw).FirstOrDefault();
            if(resultUser != null)
            {
	            HttpContext.Session.SetString("Login", "True");
                HttpContext.Session.SetString("UserId", resultUser.Id.ToString());
                ViewData["ClientId"] = HttpContext.Session.GetString("UserId");
                HttpContext.Session.SetString("UserType", resultUser.TypeOfUser.ToString());
                resultUser.LastLogin = DateTime.Now;
                _dataProvider.SaveChanges();
                return RedirectToAction("Index","Home");
                //show alert
            }
            ViewBag.ErrorMessage = "Helytelen email vagy jelszó";
            return View("Welcome");
        }

        [HttpPost]
        public ActionResult SendVerificationEmail(string email)
        {
            try
            {
                var isThereAnyEqualEmails = _dataProvider.Users.Where(x => x.Email == email).FirstOrDefault();
                if (isThereAnyEqualEmails is not null)
                {
                    return Json(new { success = false, message = "Foglalt email" });
                }
                if (email is null)
                {
					return Json(new { success = false, message = "Adjon meg egy létező emailt" });
				}
                if (!EmailVerificator.IsValidEmail(email))
                {
                    return Json(new { success = false, message = "Helytelen email" });
                }
                // Your logic to send the verification email
                Random random = new Random();
                sessionVerificationCode = random.Next(100000,1000000).ToString();
                EmailVerificator.SendVerificationEmail(email, sessionVerificationCode);

                // Return a success response
                return Json(new { success = true, message = "Ellenőrző kód elküldve" });
            }
            catch (Exception ex)
            {
                // Return an error response if something goes wrong
                return Json(new { success = false, message = "Hiba az ellenőrző kód küldésénél: " + ex.Message });
            }
        }
    }
}
