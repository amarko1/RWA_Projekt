using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class VideoController : Controller
    {
        private readonly IVideoRepository _videoRepository;

        public IActionResult AllVideos()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllVideos()
        {
            var allVideos = _videoRepository.GetAll();

            return Json(new
            {
                data = allVideos
            });
        }

    }
}
