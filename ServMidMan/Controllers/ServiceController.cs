using Microsoft.AspNetCore.Mvc;
using ServMidMan.Data;
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
            return View();
        }

        public IActionResult SendRequest(Product productId)
        {
            Service service = new Service()
            {
                Approved = false,
                Price = productId.Price,
                ProductId = productId.Id,
                UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"))
            };

            _dataProvider.Services.Add(service);
            _dataProvider.SaveChanges();
            return View();
        }

    }
}
