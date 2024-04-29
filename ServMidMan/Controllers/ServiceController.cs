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
        public IActionResult Index(bool wantAllServices = true)
        {
            if (!SiteGuardian.CheckSession(HttpContext))
            {
                return RedirectToAction("Welcome", "Authentication");
            }
            ViewData["typeOfUser"] = HttpContext.Session.GetString("UserType");

            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            ViewData["ClientId"] = HttpContext.Session.GetString("UserId");
            var myProducts = _dataProvider.Products.Where(x => x.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId")))
                .Select(x => x.Id)
                .ToList();
            List<Service> serviceList = new List<Service>();
            ServicesOrdered servicesOrdered = new ServicesOrdered();
            if (ViewData["typeOfUser"].Equals("Client"))
            {
                foreach (var product in myProducts)
                {
                    serviceList = _dataProvider.Services.Where(service => service.ProductId == product)
                        .ToList();
                    ServiceWithProduct serviceWithProduct = new ServiceWithProduct();
                    foreach (var service in serviceList)
                    {
                        var foundProduct = _dataProvider.Products.FirstOrDefault(x => x.Id == product);
                        serviceWithProduct.OwnerName = _dataProvider.Users.FirstOrDefault(x => x.Id == foundProduct.UserId).Name;
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
                var myServices = _dataProvider.Services.Where(x => x.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId"))).ToList();
                foreach (var service in myServices)
                {
                    ServiceWithProduct serviceWithProduct = new ServiceWithProduct();
                    serviceWithProduct.service = service;
                    serviceWithProduct.product.Products = _dataProvider.Products.Where(x => x.Id == service.ProductId).FirstOrDefault();
                    serviceWithProduct.OwnerName = _dataProvider.Users.FirstOrDefault(x => x.Id == Convert.ToInt32(HttpContext.Session.GetString("UserId"))).Name;
                    serviceWithProduct.product.ImagePaths = ImageOperator.getImageFullPath(_dataProvider.Images.Where(x => x.ProductReferenceId == serviceWithProduct.product.Products.Id).Select(x => x.FileName).ToList());
                    servicesOrdered.Services.Add(serviceWithProduct);
                }

            }
            if (servicesOrdered.Services.Count == 0)
            {
                ViewBag.Services = "Nincsnek létező kérések";
                return View();
            }
            if(!wantAllServices)
            {
                servicesOrdered.Services = servicesOrdered.Services.Where(x=>x.service.Approved != ServiceStatus.Done).ToList();
            }
            ViewData["AllServiceCheckbox"] = wantAllServices;
            return View(servicesOrdered);
        }

        public IActionResult SendRequest(Product productId, DateTime dateTimeToFinish)
        {
            ViewData["typeOfUser"] = HttpContext.Session.GetString("UserType");
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            Service service = new Service()
            {
                Approved = ServiceStatus.UnSettled,
                Price = productId.Price,
                ProductId = productId.Id,
                UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId")),
                Description = productId.Description,
                ApproximetlyFinishDate = dateTimeToFinish,
            };

            _dataProvider.Services.Add(service);
            _dataProvider.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Approve(int serviceId)
        {
            // Implement the logic to approve the service with the given serviceId
            var myAprrovedService = _dataProvider.Services.Where(x => x.Id == serviceId).FirstOrDefault();
            var ProductId = _dataProvider.Products.Where(x => x.Id == myAprrovedService.ProductId).Select(x => x.Id).FirstOrDefault();
            myAprrovedService.Approved = ServiceStatus.Approved;
            _dataProvider.Services.Where(_x => _x.ProductId == ProductId).ToList().ForEach(x =>
            {
                if (x.Approved != ServiceStatus.Approved)
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
        [HttpGet]
        public IActionResult UpdateProductStatus(string productStatus, int serviceId)
        {
            Service myService = _dataProvider.Services.Where(x => x.Id == serviceId).FirstOrDefault();
            myService.productStatus = (ProductStatus)Enum.Parse(typeof(ProductStatus), productStatus);
            if (myService.productStatus == ProductStatus.Done)
            {
                myService.Approved = ServiceStatus.Done;

            }
            _dataProvider.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

