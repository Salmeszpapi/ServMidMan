using Microsoft.AspNetCore.Builder;
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
                user.Password = PasswordHasher.HashPassword(user.Password);
                var result = _dataProvider.Users.Add(user as User);
                _dataProvider.SaveChanges();
                HttpContext.Session.SetString("Login", "True");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Already taken email";
            }
            return RedirectToAction("Register");
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            var pw = PasswordHasher.HashPassword(user.Password);
            var result = _dataProvider.Users.Where(x=>x.Email == user.Email && x.Password == pw).FirstOrDefault();
            if(result != null)
            {
	            HttpContext.Session.SetString("Login", "True");
				return RedirectToAction("Index","Home");
                //show alert
            }
            TempData["ErrorMessage"] = "Invalid Username or Password ";
            return RedirectToAction("Welcome");
        }
    }

}
