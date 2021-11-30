namespace CarsInfoApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Token = token;
            }
            return View();
        }
    }
}
