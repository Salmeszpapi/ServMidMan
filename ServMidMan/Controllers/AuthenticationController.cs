using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServMidMan.Data;
using ServMidMan.Models;
using ServMidMan.Services;

namespace ServMidMan.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly DataProviderContext _dataProvider;
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
        public IActionResult RegisterHandling(UserWithRegister user)
        {
            try
            {
                var isThereAnyEqualEmails = _dataProvider.Users.Where(x => x.Email == user.Email).FirstOrDefault();
                if(isThereAnyEqualEmails is not null)
                {
                    ViewBag.ErrorMessage = "Already taken email";
                    return View("Register");
                }

                user.Password = PasswordHasher.HashPassword(user.Password);
                user.LastLogin = DateTime.Now;
                _dataProvider.Users.Add(user as User);
                _dataProvider.SaveChanges();
                var result = _dataProvider.Users.Where(x=>x.Email == user.Email && x.Name == user.Name).FirstOrDefault();
                HttpContext.Session.SetString("Login", "True");
                HttpContext.Session.SetString("UserId", result.Id.ToString());
                HttpContext.Session.SetString("UserType", result.TypeOfUser.ToString());
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Already taken email";
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
                HttpContext.Session.SetString("UserType", resultUser.TypeOfUser.ToString());
                resultUser.LastLogin = DateTime.Now;
                _dataProvider.SaveChanges();
                return RedirectToAction("Index","Home");
                //show alert
            }
            ViewBag.ErrorMessage = "Invalid Username or Password ";
            return View("Welcome");
        }
    }

}
