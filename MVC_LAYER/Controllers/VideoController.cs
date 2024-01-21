using DATA_LAYER.BLModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace MVC_LAYER.Controllers
{
    public class VideoController : Controller
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IGenreRepository _genreRepository;

        public VideoController(IVideoRepository videoRepository, IGenreRepository genreRepository)
        {
            _videoRepository = videoRepository;
            _genreRepository = genreRepository;
        }

        public IActionResult Search(string searchText, bool search, int? dataPage, int? dataSize = 10)
        {
            var cookiesSearchText = Request.Cookies["searchText"];

            if (search == false && string.IsNullOrEmpty(searchText))
            {
                searchText = cookiesSearchText;
            }
            else if (search == true && searchText != null)
            {
                Response.Cookies.Append("searchText", searchText);
            }
            else if (search == true && searchText == null)
            {
                Response.Cookies.Delete("searchText");
            }

            ViewBag.SearchText = searchText;


            ViewBag.DataPage = dataPage;
            ViewBag.DataSize = dataSize;
            ViewBag.DataSizes = GetSizes();

            (var allVideos, var unpagedCount) = _videoRepository.SearchVideos(searchText, dataPage, dataSize);

            ViewBag.TotalPages = unpagedCount / dataSize;


            return View(allVideos);
        }

        private List<SelectListItem> GetSizes() =>
            new List<SelectListItem>
            {
                new SelectListItem{Value = "5", Text = "5"},
                new SelectListItem{Value = "10", Text = "10"},
                new SelectListItem{Value = "20", Text = "20"},
                new SelectListItem{Value = "50", Text = "50"},
                new SelectListItem{Value = "100", Text = "100"},
            };


        public IActionResult AllVideos()
        {
            var allVideos = _videoRepository.GetAll();

            return View(allVideos);
        }

        public IActionResult CreateVideo()
        {
            SetPeopleToView();

            return View();
        }

        private void SetPeopleToView()
        {
            var genres = _genreRepository.GetAll();

            var selectListItems = genres.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            });

            ViewBag.GenreSelectList = new SelectList(selectListItems, "Value", "Text");
        }

        [HttpPost]
        public IActionResult CreateVideo(BLVideo blVideo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Log ModelState errors
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            var errorMessage = error.ErrorMessage;
                            Debug.WriteLine($"Validation Error: {errorMessage}");
                        }
                    }
                    return View(blVideo);   
                }

                _videoRepository.Add(blVideo);

                return RedirectToAction("Search");
            }
            catch
            {
                return RedirectToAction("Search");
            }
        }


        public IActionResult EditVideo(int id)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound();
                }

                var videoForEdit = _videoRepository.Get(id);

                if (videoForEdit == null)
                {
                    return NotFound();
                }
                SetPeopleToView();
                return View(videoForEdit);
            }
            catch
            {
                return RedirectToAction("Search");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditVideo(BLVideo video)
        {
            try
            {
                if (!ModelState.IsValid) return View(video);

                int id = video.Id;

                _videoRepository.Modify(id, video);

                return RedirectToAction("Search");
            }
            catch
            {
                return RedirectToAction("Search");
            }
        }

        //nalodaj view s odabranim videom
        public IActionResult DeleteVideo(int id, BLGenre genre) //fix
        {
            try
            {
                if (id == 0)
                {
                    return NotFound();
                }

                var videoForDelete = _videoRepository.Get(id);

                if (videoForDelete == null)
                {
                    return NotFound();
                }

                return View(videoForDelete);
            }
            catch
            {
                return RedirectToAction("Search");
            }
        }

        //obrisi ga
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteVideo(int id)
        {
            try
            {
                if (id == 0) return NotFound();

                var deletedVideo = _videoRepository.Remove(id);

                if (deletedVideo == null)
                    return Json(new { success = false, message = "Unable to delete video" });

                return RedirectToAction("Search");
            }
            catch
            {
                return RedirectToAction("Search");
            }
        }
    }
}
