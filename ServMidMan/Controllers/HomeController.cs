using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using ServMidMan.Data;
using ServMidMan.Helper;
using ServMidMan.Hubs;
using ServMidMan.Models;
using ServMidMan.Services;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace ServMidMan.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataProviderContext _dataProvider;
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<ChatHub> _hubContext;
        public HomeController(ILogger<HomeController> logger, DataProviderContext dataProviderContext, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _dataProvider = dataProviderContext;
        }

        public IActionResult Index(SearchProducts filteredProducts = null)
        {
            if (!SiteGuardian.CheckSession(HttpContext))
            {
                return RedirectToAction("Welcome", "Authentication");
            }
            var products = ProductsFilter(filteredProducts);


            List<Byte[]> bytes = new List<Byte[]>();
            List<ProductWithByteImages> myProductWithByteImages = new List<ProductWithByteImages>();
            foreach (var product in products)
            {
                var myImages = _dataProvider.Images.Where(x => x.ProductReferenceId == product.Id).Select(x=>x.FileName).ToList();
                myProductWithByteImages.Add(new ProductWithByteImages
                {
                    Products =  product ,
                    //ImageResources =  ImageOperator.DownloadImages(myImages),
                    ImagePaths = ImageOperator.getImageFullPath(myImages),
                });
            }
            if (products.Count == 0)
            {
                ViewBag.NoProduct = "No existing products";
                return View();
            }
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            ViewData["typeOfUser"] = HttpContext.Session.GetString("UserType");

            return View(myProductWithByteImages);
        }


        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Product(string id)
        {
            ViewData["typeOfUser"] = HttpContext.Session.GetString("UserType");

            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            ViewData["ClientId"] = HttpContext.Session.GetString("UserId");
            Product product = _dataProvider.Products.Where(x => x.Id.ToString() == id).FirstOrDefault();
            if(product == null)
            {
                ViewBag.Error = "Product not find";
                return View();
            }
            var myImages = _dataProvider.Images.Where(x => x.ProductReferenceId == product.Id).Select(x => x.FileName).ToList();
            ProductWithByteImages myProductWithByteImages = new ProductWithByteImages();
            myProductWithByteImages = new ProductWithByteImages
            {
                Products = product,
                //ImageResources = ImageOperator.DownloadImages(myImages),
                ImagePaths = ImageOperator.getImageFullPath(myImages)
            };
            return View(myProductWithByteImages);
        }
        public IActionResult Logout()
        {
            ViewBag.LoggedIn = null;
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
            ImageOperator.ImageUploaderToServer(files, myImagesToPush);

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
                ImageOperator.ImageUploaderToServer(files, myImagesToPush);
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

        public IActionResult Profile()
        {
            if (!SiteGuardian.CheckSession(HttpContext))
            {
                return RedirectToAction("Welcome", "Authentication");
            }

            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            ViewData["ClientId"] = HttpContext.Session.GetString("UserId");
            ViewData["typeOfUser"] =HttpContext.Session.GetString("UserType");
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var myProducts = _dataProvider.Products.Where(x=>x.UserId == userId).ToList();
            ProductWithImagesPathAndUserInfo productWithImagesPathAndUserInfo = new ProductWithImagesPathAndUserInfo();
            productWithImagesPathAndUserInfo.UserInfo = _dataProvider.Users.Where(x=>x.Id == userId).FirstOrDefault();
            foreach (var product in myProducts)
            {
                var myImages = _dataProvider.Images.Where(x => x.ProductReferenceId == product.Id).Select(x => x.FileName).ToList();
                productWithImagesPathAndUserInfo.productWithByteImages.Add(new ProductWithByteImages
                {
                    Products = product,
                    //ImageResources =  ImageOperator.DownloadImages(myImages),
                    ImagePaths = ImageOperator.getImageFullPath(myImages),
                });
            }

            return View(productWithImagesPathAndUserInfo);
        }

        [HttpPost]
        public IActionResult ProfileUpdate(UserWithRegister user)
        {
            if (!SiteGuardian.CheckSession(HttpContext))
            {
                return RedirectToAction("Welcome", "Authentication");
            }
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            ViewData["ClientId"] = HttpContext.Session.GetString("UserId");
            ViewData["typeOfUser"] = HttpContext.Session.GetString("UserType");
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            User myUser = _dataProvider.Users.FirstOrDefault(c => c.Id == userId);
            if((user.Password == user.Password2) && !string.IsNullOrWhiteSpace(user.Password))
            {
                myUser.Password = PasswordHasher.HashPassword(user.Password);
            }
            myUser.Name = user.Name;

            _dataProvider.SaveChanges();

            var myProducts = _dataProvider.Products.Where(x => x.UserId == userId).ToList();
            ProductWithImagesPathAndUserInfo productWithImagesPathAndUserInfo = new ProductWithImagesPathAndUserInfo();
            productWithImagesPathAndUserInfo.UserInfo = myUser;
            foreach (var product in myProducts)
            {
                var myImages = _dataProvider.Images.Where(x => x.ProductReferenceId == product.Id).Select(x => x.FileName).ToList();
                productWithImagesPathAndUserInfo.productWithByteImages.Add(new ProductWithByteImages
                {
                    Products = product,
                    //ImageResources =  ImageOperator.DownloadImages(myImages),
                    ImagePaths = ImageOperator.getImageFullPath(myImages),
                });
            }
            //checking 
            return View("Profile", productWithImagesPathAndUserInfo);
        }

        [HttpPost]
        public IActionResult ProfileRatingUpdate(int rating)
        {
            if (!SiteGuardian.CheckSession(HttpContext))
            {
                return RedirectToAction("Welcome", "Authentication");
            }
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            ViewData["ClientId"] = HttpContext.Session.GetString("UserId");
            ViewData["typeOfUser"] = HttpContext.Session.GetString("UserType");
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            User myUser = _dataProvider.Users.FirstOrDefault(c => c.Id == userId);
            myUser.Voters += 1;
            myUser.Rating = myUser.Rating + rating;
            _dataProvider.SaveChanges();
            var myProducts = _dataProvider.Products.Where(x => x.UserId == userId).ToList();
            ProductWithImagesPathAndUserInfo productWithImagesPathAndUserInfo = new ProductWithImagesPathAndUserInfo();
            productWithImagesPathAndUserInfo.UserInfo = myUser;
            foreach (var product in myProducts)
            {
                var myImages = _dataProvider.Images.Where(x => x.ProductReferenceId == product.Id).Select(x => x.FileName).ToList();
                productWithImagesPathAndUserInfo.productWithByteImages.Add(new ProductWithByteImages
                {
                    Products = product,
                    //ImageResources =  ImageOperator.DownloadImages(myImages),
                    ImagePaths = ImageOperator.getImageFullPath(myImages),
                });
            }
            //checking 
            return View("Profile", productWithImagesPathAndUserInfo);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Upload()
        {
            ViewData["typeOfUser"] =HttpContext.Session.GetString("UserType");
            if (!SiteGuardian.CheckSession(HttpContext))
            {
                return RedirectToAction("Welcome", "Authentication");
            }
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            return View();
        }

        private List<Product> ProductsFilter(SearchProducts searchProducts)
        {
            List<Product> products = null;
            if (searchProducts.Name != null)
            {
                products = _dataProvider.Products.Where(x => x.Name == searchProducts.Name).ToList();
            }
            if (searchProducts.Category != null)
            {
                if(products == null)
                {
                    products = _dataProvider.Products.Where(x => x.Category == searchProducts.Category).ToList();
                }
                else
                {
                    products.Where(x => x.Category == searchProducts.Category).ToList();
                }
                
            }
            if (searchProducts.Location != null)
            {
                if (products == null)
                {
                    products = _dataProvider.Products.Where(x => x.Location == searchProducts.Location).ToList();
                }
                else
                {
                    products.Where(x => x.Location == searchProducts.Location).ToList();
                }
                
            }
            if (searchProducts.LocationAround != null)
            {
                //TODO
            }
            if (searchProducts.MinPrice != null)
            {
                if (products == null)
                {
                    products = _dataProvider.Products.Where(x => x.Price >= searchProducts.MinPrice).ToList();
                }
                else
                {
                    products.Where(x => x.Price >= searchProducts.MinPrice).ToList();
                }
            }
            if (searchProducts.Price != null)
            {
                if (products == null)
                {
                    products = _dataProvider.Products.Where(x => x.Price <= searchProducts.Price).ToList();
                }
                else
                {
                    products.Where(x => x.Price <= searchProducts.Price).ToList();
                }
            }



            return products;
        }
    }
}
