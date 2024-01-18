using Microsoft.AspNetCore.Mvc;

namespace MVC_LAYER.Controllers
{
    public class VideoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
