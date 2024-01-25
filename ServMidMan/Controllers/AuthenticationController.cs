using Microsoft.AspNetCore.Mvc;
using ServMidMan.Data;

namespace ServMidMan.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Index(IApplicationBuilder applicationBuilder)
        {
            using(var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DataProviderContext>();
                var result = context.Database.EnsureCreated();
            }

            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}
