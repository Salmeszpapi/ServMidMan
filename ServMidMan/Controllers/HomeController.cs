using Microsoft.AspNetCore.Mvc;
using ServMidMan.Data;
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
	        List<Product> products = _dataProvider.Products.ToList() ;
	        ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
			return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Upload()
        {
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
