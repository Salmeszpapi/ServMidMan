using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using ServMidMan.Data;
using ServMidMan.Helper;
using ServMidMan.Models;
using System.Diagnostics;
using System.Net;

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
            SiteGuardian.ClientType = HttpContext.Session.GetString("UserType");
            List<Product> products = _dataProvider.Products.ToList();

            List<Byte[]> bytes = new List<Byte[]>();
            List<ProductWithByteImages> myProductWithByteImages = new List<ProductWithByteImages>();
            foreach (var product in products)
            {
                var myImages = _dataProvider.Images.Where(x => x.ProductReferenceId == product.Id).Select(x=>x.FileName).ToList();
                myProductWithByteImages.Add(new ProductWithByteImages
                {
                    Products =  product ,
                    ImageResources =  ImageOperator.DownlaodImages(myImages),
                });
            }

            var typeOfUser = HttpContext.Session.GetString("UserType");
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            ViewData["typeOfUser"] = typeOfUser;

            return View(myProductWithByteImages);
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
            ViewData["ClientId"] = HttpContext.Session.GetString("UserId");
            Product product = _dataProvider.Products.Where(x => x.Id.ToString() == id).FirstOrDefault();
            var myImages = _dataProvider.Images.Where(x => x.ProductReferenceId == product.Id).Select(x => x.FileName).ToList();
            ProductWithByteImages myProductWithByteImages = new ProductWithByteImages();
            myProductWithByteImages = new ProductWithByteImages
            {
                Products = product,
                ImageResources = ImageOperator.DownlaodImages(myImages),
            };

            return View(myProductWithByteImages);
        }
        public IActionResult Logout()
        {
            return RedirectToAction("Welcome", "Authentication");
        }
        [HttpPost]
        public IActionResult NewProduct(Product product, [FromForm(Name = "fileInput")] List<IFormFile> files)
        {
            int lastId = 0;
            if (_dataProvider.Products.Any())
            {
                lastId = _dataProvider.Products.Max(p => p.Id)+1;
            }
            var userId = HttpContext.Session.GetString("UserId");
            product.UserId = Convert.ToInt32(userId);
            _dataProvider.Products.Add(product);
            List<Image> myImagesToPush = new List<Image>();
            foreach (IFormFile file in files) 
            {
                Guid guid = Guid.NewGuid();
                Image myImage = new Image()
                {
                    FileName = guid + file.FileName,
                    ProductReferenceId = lastId
                };
                myImagesToPush.Add(myImage);
                _dataProvider.Images.Add(myImage);
            }
            _dataProvider.SaveChanges();
            ImageOperator.imageUploaderToServer(files, myImagesToPush);
            return RedirectToAction("Index");
        }
        public IActionResult UpdateProduct(Product product, [FromForm(Name = "fileInput")] List<IFormFile> files)
        {
            product.UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            Product dbProduct = _dataProvider.Products.FirstOrDefault(c => c.Id == product.Id);
            if (dbProduct != null)
            {
                dbProduct.Name = product.Name;
                dbProduct.Price = product.Price;
                dbProduct.Location = product.Location;
                dbProduct.Description = product.Description;
                dbProduct.Location = product.Location.ToString();
                dbProduct.Category = product.Category;
                // Update other properties as needed
                dbProduct.UserId = product.UserId; // Make sure to update the UserId if needed
            }

            List<Image> myImagesToPush = new List<Image>();
            foreach (IFormFile file in files)
            {
                Guid guid = Guid.NewGuid();
                Image myImage = new Image()
                {
                    FileName = guid + file.FileName,
                    ProductReferenceId = dbProduct.Id
                };
                myImagesToPush.Add(myImage);
                _dataProvider.Images.Add(myImage);
            }
            _dataProvider.SaveChanges();
            if(files.Count != 0)
            {
                ImageOperator.imageUploaderToServer(files, myImagesToPush);
            }
            return RedirectToAction("Index");
        }

        public IActionResult DeleteProduct(Product product, [FromForm(Name = "fileInput")] List<IFormFile> files)
        {
            product.UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            Product dbProduct = _dataProvider.Products.FirstOrDefault(c => c.Id == product.Id);
            List<Image> images= _dataProvider.Images.Where(c => c.ProductReferenceId == dbProduct.Id).ToList();
            List<string> imageNames = images.Select(x => x.FileName).ToList();
            ImageOperator.FTPImgaeRemover(imageNames);
            foreach(Image image in images)
            {
                _dataProvider.Images.Remove(image);

            }
            _dataProvider.Products.Remove(dbProduct);
            _dataProvider.SaveChanges();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
    }
}
