using DATA_LAYER.BLModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepository;

        public GenreController(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public IActionResult AllGenres()
        {
            var allGenres = _genreRepository.GetAll();

            return View(allGenres);
        }

        public IActionResult CreateGenre()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateGenre(BLGenre blGenre)
        {
            try
            {
                if (!ModelState.IsValid) return View(blGenre);

                _genreRepository.Add(blGenre);

                return RedirectToAction("AllGenres");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }


        public IActionResult EditGenre(int id)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound();
                }

                var tagForEdit = _genreRepository.Get(id);

                if (tagForEdit == null)
                {
                    return NotFound();
                }

                return View(tagForEdit);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditGenre(BLGenre genre)
        {
            try
            {
                if (!ModelState.IsValid) return View(genre);

                int id = genre.Id;

                _genreRepository.Modify(id, genre);

                return RedirectToAction("AllGenres");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }

        [HttpDelete]
        public IActionResult DeleteGenre(int id)
        {
            try
            {
                if (id == 0) return NotFound();

                var deletedGenre = _genreRepository.Remove(id);

                if (deletedGenre == null)
                    return Json(new { success = false, message = "Unable to delete genre" });

                return Json(new { success = true, message = "Genre deleted successfully" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }
    }
}
