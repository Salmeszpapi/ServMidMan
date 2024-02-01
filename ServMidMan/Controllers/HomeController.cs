using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using ServMidMan.Data;
using ServMidMan.Helper;
using ServMidMan.Models;
using System.Diagnostics;

namespace ServMidMan.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataProviderContext _dataProvider;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataProviderContext dataProviderContext)
        {
            _logger = logger;
            _dataProvider = dataProviderContext;
        }

        public IActionResult Index()
        {
            if (!SiteGuardian.CheckSession(HttpContext))
            {
                return RedirectToAction("Welcome", "Authentication");
            }
            var typeOfUser = HttpContext.Session.GetString("UserType");
            List<Product> products = _dataProvider.Products.ToList() ;
	        ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
	        ViewData["typeOfUser"] = typeOfUser;
			return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Product(string id)
        {
	        List<Product> products = _dataProvider.Products.Where(x=>x.Id.ToString() == id).ToList();
			return View(products);
        }
		public IActionResult Upload()
        {
            if (!SiteGuardian.CheckSession(HttpContext))
            {
                return RedirectToAction("Welcome", "Authentication");
            }
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            return View();
        }
        public IActionResult Logout()
        {
			return RedirectToAction("Welcome", "Authentication");
		}
		[HttpPost]
        public IActionResult NewProduct(Product product, [FromForm(Name = "fileInput")] List<IFormFile> file)
        {
            var userId = HttpContext.Session.GetString("UserId");
            product.UserId = Convert.ToInt32(userId);
            _dataProvider.Products.Add(product);
            _dataProvider.SaveChanges();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
