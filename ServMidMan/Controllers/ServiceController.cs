﻿using Microsoft.AspNetCore.Mvc;
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
            List<Service> serviceList = new List<Service>();
            foreach (var product in myProducts) {
                serviceList = _dataProvider.Services.Where(service => service.ProductId == product)
                    .ToList();
                foreach (var service in serviceList)
                {
                    servicesDb.Add(service);
                }
            }

            // check if the product is mine 

            //return request whery userId is mine 

            ServicesOrdered servicesOrdered = new ServicesOrdered();
            servicesOrdered.SenderServices = _dataProvider.Services.Where(x=>x.UserId == SiteGuardian.CurrentClientId).ToList();
            if(servicesDb.Count == 0 && servicesOrdered.SenderServices.Count == 0)
            {
                ViewBag.Services = "No existing requests";
                return View();
            }
            foreach (Service service in servicesDb)
            {
                servicesOrdered.ReceivedServices = servicesDb;
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
            ViewData["typeOfUser"] = SiteGuardian.ClientType;
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");

            var myservice = _dataProvider.Services.Where(x => x.Id == inputService.Id).FirstOrDefault();

            _dataProvider.Services.Remove(myservice);
            _dataProvider.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Deletev2(int serviceId)
        {
            ViewData["typeOfUser"] = SiteGuardian.ClientType;
            ViewData["LoggedIn"] = HttpContext.Session.GetString("Login");

            var myservice = _dataProvider.Services.Where(x => x.Id == serviceId).FirstOrDefault();

            _dataProvider.Services.Remove(myservice);
            _dataProvider.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
