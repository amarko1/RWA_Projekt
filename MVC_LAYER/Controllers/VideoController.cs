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

        //[HttpPost]
        //public IActionResult CreateVideo(BLVideo blVideo)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            // Log ModelState errors
        //            foreach (var modelState in ModelState.Values)
        //            {
        //                foreach (var error in modelState.Errors)
        //                {
        //                    var errorMessage = error.ErrorMessage;
        //                    Debug.WriteLine($"Validation Error: {errorMessage}");
        //                }
        //            }
        //            return View(blVideo);   
        //        }

        //        _videoRepository.Add(blVideo);

        //        return RedirectToAction("Search");
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Search");
        //    }
        //}
        //[HttpPost]
        //public async Task<IActionResult> CreateVideo(BLVideo blVideo)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            // ... handle invalid model ...
        //            return View(blVideo);
        //        }

        //        // Handle the file upload
        //        if (blVideo.VideoImage != null)
        //        {
        //            var folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "images");
        //            Directory.CreateDirectory(folderPath); // Ensure the directory exists

        //            var uniqueFileName = Guid.NewGuid().ToString() + "_" + blVideo.VideoImage.FileName;
        //            var filePath = Path.Combine(folderPath, uniqueFileName);

        //            using (var fileStream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await blVideo.VideoImage.CopyToAsync(fileStream);
        //            }

        //            blVideo.ImagePath = uniqueFileName; // Save this path to the database
        //        }

        //        _videoRepository.Add(blVideo);

        //        return RedirectToAction("Search");
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Search");
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> CreateVideo(BLVideo blVideo)
        {
            if (ModelState.IsValid)
            {
                int? imageId = null; // Bit će null ako nema slike

                // Provjerite da li postoji učitana slika
                if (blVideo.VideoImage != null)
                {
                    // Spremite sliku i dobijte putanju
                    var imagePath = await SaveImage(blVideo.VideoImage);

                    // Stvorite BLImage instancu
                    var blImage = new BLImage { Content = imagePath };

                    // Spremite BLImage i dobijte ID
                    imageId = await _imageRepository.AddImageAsync(blImage);
                }

                // Sada kada imate imageId, možete stvoriti i sačuvati video zapis
                // Pretpostavimo da imate neku metodu AddVideoAsync unutar video repozitorija
                var blVideoEntity = new BLVideo
                {
                    Name = blVideo.Name,
                    Description = blVideo.Description,
                    GenreId = blVideo.GenreId,
                    TotalSeconds = blVideo.TotalSeconds,
                    StreamingUrl = blVideo.StreamingUrl,
                    ImageId = imageId // Postavite ID slike ako je slika učitana
                };

                 _videoRepository.Add(blVideoEntity);

                return RedirectToAction("Search");
            }

            // Ako model nije valjan, prikažite formu ponovo s greškama
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

        //private async Task<int> SaveImageRecord(string imagePath)
        //{
        //    // Ovdje stvarate novi zapis u tablici 'Image' s putanjom do slike
        //    var imageRecord = new BLImage // Pretpostavljam da imate model koji odgovara vašoj tablici 'Image'
        //    {
        //        Content = Path.GetFileName(imagePath) // Spremite samo ime datoteke ako želite
        //    };

        //    await _imageRepository.AddImageAsync(imageRecord); // Pretpostavljam da repozitorij ima metodu 'Add'
        //    await _imageRepository.SaveChangesAsync(); // Spremite promjene u bazi podataka

        //    return imageRecord.Id; // Vratite ID novog zapisa
        //}


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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult EditVideo(BLVideo video)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid) return View(video);

        //        int id = video.Id;

        //        _videoRepository.Modify(id, video);

        //        return RedirectToAction("Search");
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Search");
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVideo(BLVideo blVideo, IFormFile? videoImage)
        {
            try
            {
                if (!ModelState.IsValid) return View(blVideo);

                // Initialize imageId with the current image ID of the video
                int? imageId = blVideo.ImageId;

                // Check if a new image was uploaded
                if (videoImage != null && videoImage.Length > 0)
                {
                    // Save the new image and get its path
                    var imagePath = await SaveImage(videoImage);

                    // Create BLImage instance
                    var blImage = new BLImage { Content = imagePath };

                    // Save BLImage and get ID
                    imageId = await _imageRepository.AddImageAsync(blImage);
                }

                // Get the current video from the repository (to update it)
                var videoToUpdate =  _videoRepository.Get(blVideo.Id);
                if (videoToUpdate != null)
                {
                    // Update video details
                    videoToUpdate.Name = blVideo.Name;
                    videoToUpdate.Description = blVideo.Description;
                    videoToUpdate.GenreId = blVideo.GenreId;
                    videoToUpdate.TotalSeconds = blVideo.TotalSeconds;
                    videoToUpdate.StreamingUrl = blVideo.StreamingUrl;
                    videoToUpdate.ImageId = imageId; // Set to the new imageId if a new image was uploaded, else remains the same

                    // Update the video record in the repository
                    _videoRepository.Modify(videoToUpdate.Id, videoToUpdate);
                }

                return RedirectToAction("Search");
            }
            catch
            {
                // Return to the edit view if something goes wrong
                return View(blVideo);
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
