using Microsoft.AspNetCore.Mvc;
using ServMidMan.Data;
using ServMidMan.Helper;
using ServMidMan.Models;

namespace ServMidMan.Controllers
{
    public class ServiceController : Controller
    {
        private ILogger<HomeController> _logger;
        private DataProviderContext _dataProvider;

        public ServiceController(ILogger<HomeController> logger, DataProviderContext dataProviderContext)
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
            ViewData["typeOfUser"] = HttpContext.Session.GetString("UserType");

            var myProducts = _dataProvider.Products.Where(x => x.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId")))
                .Select(x=>x.Id)
                .ToList();
            List<Service> serviceList = new List<Service>();
            ServicesOrdered servicesOrdered = new ServicesOrdered();
            if (ViewData["typeOfUser"] == "Client")
            {
                foreach (var product in myProducts) {
                    serviceList = _dataProvider.Services.Where(service => service.ProductId == product)
                        .ToList();
                    ServiceWithProduct serviceWithProduct = new ServiceWithProduct();
                    foreach (var service in serviceList)
                    {
                        var foundProduct = _dataProvider.Products.FirstOrDefault(x => x.Id == product);
                        
                        serviceWithProduct.product.Products = foundProduct;
                        serviceWithProduct.product.ImagePaths = ImageOperator.getImageFullPath(_dataProvider.Images.Where(x => x.ProductReferenceId == serviceWithProduct.product.Products.Id).Select(x => x.FileName).ToList());
                        serviceWithProduct.service = service;
                    }
                    if (serviceList.Count > 0)
                    {
                        servicesOrdered.Services.Add(serviceWithProduct);

                    }
                }
            }
            else
            {
                ServiceWithProduct serviceWithProduct = new ServiceWithProduct();
                   var myServices =  _dataProvider.Services.Where(x => x.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId"))).ToList();
                foreach(var service in myServices)
                {
                    serviceWithProduct.service = service;
                    serviceWithProduct.product.Products = _dataProvider.Products.Where(x=>x.Id == service.ProductId).FirstOrDefault();
                    serviceWithProduct.product.ImagePaths = ImageOperator.getImageFullPath(_dataProvider.Images.Where(x => x.ProductReferenceId == serviceWithProduct.product.Products.Id).Select(x => x.FileName).ToList());
                    servicesOrdered.Services.Add(serviceWithProduct);
                }

            }
            if (servicesOrdered.Services.Count == 0)
            {
                ViewBag.Services = "No existing requests";
                return View();
            }
            return View(servicesOrdered);
        }

        public IActionResult SendRequest(Product productId)
        {
            ViewData["typeOfUser"] = HttpContext.Session.GetString("UserType");
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            Service service = new Service()
            {
                Approved = false,
                Price = productId.Price,
                ProductId = productId.Id,
                UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId")),
                Description = productId.Description,
            };

            _dataProvider.Services.Add(service);
            _dataProvider.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Approve(int serviceId)
        {
            // Implement the logic to approve the service with the given serviceId
            var myAprrovedService = _dataProvider.Services.Where(x=>x.Id == serviceId).FirstOrDefault();
            var ProductId = _dataProvider.Products.Where(x=>x.Id == myAprrovedService.ProductId).Select(x=>x.Id).FirstOrDefault();
            myAprrovedService.Approved = true;
            _dataProvider.Services.Where(_x => _x.ProductId == ProductId).ToList().ForEach(x =>
            {
                if(!x.Approved)
                {
                    _dataProvider.Services.Remove(x);
                }
            });
            _dataProvider.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(Service inputService)
        {
            ViewData["typeOfUser"] = HttpContext.Session.GetString("UserType");
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");

            var myservice = _dataProvider.Services.Where(x => x.Id == inputService.Id).FirstOrDefault();

            _dataProvider.Services.Remove(myservice);
            _dataProvider.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Deletev2(int serviceId)
        {
            ViewData["typeOfUser"] = HttpContext.Session.GetString("UserType");
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");

            var myservice = _dataProvider.Services.Where(x => x.Id == serviceId).FirstOrDefault();

            _dataProvider.Services.Remove(myservice);
            _dataProvider.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
