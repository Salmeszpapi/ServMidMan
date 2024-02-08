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
        public IActionResult Index(int productId)
        {
            ViewData["typeOfUser"] = SiteGuardian.ClientType;
            List<Service> servicesDb= new List<Service>();
            var myProducts = _dataProvider.Products.Where(x => x.UserId == SiteGuardian.CurrentClientId)
                .Select(x=>x.Id)
                .ToList();

            foreach (var product in myProducts) {
				servicesDb = _dataProvider.Services.Where(service => service.ProductId == product)
	                .ToList();
			}
            // check if the product is mine 

            //return request whery userId is mine 

            List<Service> SendedRequests = _dataProvider.Services.Where(x=>x.UserId == SiteGuardian.CurrentClientId).ToList();

            ServicesOrdered servicesOrdered = new ServicesOrdered();
            foreach (Service service in servicesDb)
            {
                if (service.UserId == SiteGuardian.CurrentClientId)
                {
                    servicesOrdered.ReceivedServices = servicesDb;
                }
                else
                {
                    servicesOrdered.SenderServices = SendedRequests;
                }
            }
            return View(servicesOrdered);
        }

        public IActionResult SendRequest(Product productId)
        {
            ViewData["typeOfUser"] = SiteGuardian.ClientType;
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");
            Service service = new Service()
            {
                Approved = false,
                Price = productId.Price,
                ProductId = productId.Id,
                UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"))
            };

            _dataProvider.Services.Add(service);
            _dataProvider.SaveChanges();
            return View("Index");
        }

    }
}
