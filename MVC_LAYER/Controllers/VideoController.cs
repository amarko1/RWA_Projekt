using DATA_LAYER.BLModels;
using DATA_LAYER.DALModels;
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
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IImageRepository _imageRepository;

        public VideoController(IImageRepository imageRepository, IVideoRepository videoRepository, IGenreRepository genreRepository, IWebHostEnvironment hostingEnvironment)
        {
            _videoRepository = videoRepository;
            _genreRepository = genreRepository;
            _hostingEnvironment = hostingEnvironment;
            _imageRepository = imageRepository;
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

        public IActionResult CardView(string searchText, bool search)
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

            ViewBag.SearchTextCardView = searchText;

            var allVideos = _videoRepository.SearchCardView(searchText);

            var videoModels = allVideos.Select(video => new BLVideo
            {
                Id = video.Id,
                Name = video.Name,
                Description = video.Description,
                GenreId = video.GenreId,
                TotalSeconds = video.TotalSeconds,
                StreamingUrl = video.StreamingUrl,
                ImagePath = video.ImageId.HasValue
                ? Path.GetFileName(_imageRepository.GetImagePathById(video.ImageId.Value))
                : null
            }).ToList();

            return View(videoModels);
        }

        public IActionResult CardDetails(int id)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound();
                }

                var videoForView = _videoRepository.Get(id);

                if (videoForView == null)
                {
                    return NotFound();
                }

                return View(videoForView);
            }
            catch
            {
                return RedirectToAction("CardView");
            }
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
            SetGenresToView();

            return View();
        }

        private void SetGenresToView()
        {
            var genres = _genreRepository.GetAll();

            var selectListItems = genres.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).OrderBy(p => p.Text);

            ViewBag.GenreSelectList = new SelectList(selectListItems, "Value", "Text");
        }

        [HttpPost]
        public async Task<IActionResult> CreateVideo(BLVideo blVideo)
        {
            if (ModelState.IsValid)
            {
                int? imageId = null; 

                
                if (blVideo.VideoImage != null)
                {
                    // save image and get path
                    var imagePath = await SaveImage(blVideo.VideoImage);

                    
                    var blImage = new BLImage { Content = imagePath };

                    // save and get id
                    imageId = await _imageRepository.AddImageAsync(blImage);
                }

                var blVideoEntity = new BLVideo
                {
                    Name = blVideo.Name,
                    Description = blVideo.Description,
                    GenreId = blVideo.GenreId,
                    TotalSeconds = blVideo.TotalSeconds,
                    StreamingUrl = blVideo.StreamingUrl,
                    ImageId = imageId 
                };

                 _videoRepository.Add(blVideoEntity);

                return RedirectToAction("Search");
            }

            return View(blVideo);
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return filePath;
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
                SetGenresToView();
                return View(videoForEdit);
            }
            catch
            {
                return RedirectToAction("Search");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVideo(BLVideo blVideo, IFormFile? videoImage)
        {
            try
            {
                if (!ModelState.IsValid) return View(blVideo);

                int? imageId = blVideo.ImageId;

                if (videoImage != null && videoImage.Length > 0)
                {
                    var imagePath = await SaveImage(videoImage);

                    var blImage = new BLImage { Content = imagePath };

                    imageId = await _imageRepository.AddImageAsync(blImage);
                }

                // Get the  video for update
                var videoToUpdate =  _videoRepository.Get(blVideo.Id);
                if (videoToUpdate != null)
                {
                    videoToUpdate.Name = blVideo.Name;
                    videoToUpdate.Description = blVideo.Description;
                    videoToUpdate.GenreId = blVideo.GenreId;
                    videoToUpdate.TotalSeconds = blVideo.TotalSeconds;
                    videoToUpdate.StreamingUrl = blVideo.StreamingUrl;
                    videoToUpdate.ImageId = imageId; 

                    _videoRepository.Modify(videoToUpdate.Id, videoToUpdate);
                }

                return RedirectToAction("Search");
            }
            catch
            {
                return View("Search");
            }
        }

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
