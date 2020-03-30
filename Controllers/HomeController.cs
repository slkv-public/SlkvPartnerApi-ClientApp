using Microsoft.AspNetCore.Mvc;

namespace SwissLife.Slkv.Partner.ClientAppSample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
