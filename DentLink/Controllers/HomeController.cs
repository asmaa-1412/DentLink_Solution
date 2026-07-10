using Microsoft.AspNetCore.Mvc;

namespace DentLink.PresentionLayer.Controllers
{
  
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
