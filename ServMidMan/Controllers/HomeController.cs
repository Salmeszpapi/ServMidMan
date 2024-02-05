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
            var typeOfUser = HttpContext.Session.GetString("UserType");
           
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            ViewData["typeOfUser"] = typeOfUser;
            Product products = _dataProvider.Products.Where(x=>x.Id.ToString() == id).FirstOrDefault();
            List<Image> images = _dataProvider.Images.Where(x => x.ProductReferenceId == products.Id.ToString()).ToList();
            ProductWithImages productWithImages = new ProductWithImages
            {
                Id = products.Id,
                Name = products.Name,
                Description = products.Description,
                Category = products.Category,
                Price = products.Price,
                UserId = products.UserId,
                Location = products.Location,
                CreatedDate = products.CreatedDate,
                Images = images
            };
            return View(productWithImages);
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
        public IActionResult NewProduct(Product product, [FromForm(Name = "fileInput")] List<IFormFile> files)
        {
            int lastId = _dataProvider.Products.Max(p => p.Id);
            var userId = HttpContext.Session.GetString("UserId");
            product.UserId = Convert.ToInt32(userId);
            _dataProvider.Products.Add(product);
            _dataProvider.SaveChanges();
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var a = new byte[1] { 0x01 };

                    IFormFile myFile = file;
                    if (file.Length > 0)
                    {
                        Image image = new Image()
                        {
                            TestImage = a,
                            ProductReferenceId = Convert.ToString(lastId),
                            FileName = myFile.FileName,
                        };
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            myFile.CopyTo(memoryStream);
                            image.ImageData = memoryStream.ToArray();
                        }
                            //var byteImg = ImageOperator.ImageToByteArray(myFile);

                        _dataProvider.Images.Add(image);
                        try
                        {
                            _dataProvider.SaveChanges();

                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
