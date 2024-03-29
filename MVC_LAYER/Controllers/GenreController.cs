﻿using DATA_LAYER.BLModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MVC_LAYER.Controllers
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult CreateGenre(BLGenre blGenre)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid) return View(blGenre);

        //        _genreRepository.Add(blGenre);

        //        return RedirectToAction("AllGenres");
        //    }
        //    catch 
        //    {
        //        return RedirectToAction("AllGenres");
        //    }
        //}

        [HttpPost]
        public IActionResult CreateGenre([FromBody] BLGenre blGenre)
        {
            try
            {
                if (!ModelState.IsValid) return View(blGenre);

                _genreRepository.Add(blGenre);

                return RedirectToAction("AllGenres");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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
            catch
            {
                return RedirectToAction("AllGenres");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditGenre(BLGenre genre)
        {
            try
            {
                if (!ModelState.IsValid) return Json(new { success = false, redirectUrl = Url.Action("AllGenres") });

                _genreRepository.Modify(genre.Id, genre);

                return Json(new { success = true, redirectUrl = Url.Action("AllGenres") });
            }
            catch
            {
                return Json(new { success = false, message = "Failed to update genre" });
            }
        }


        public IActionResult DeleteGenre(int id, BLGenre genre) //fix
        {
            try
            {
                if (id == 0)
                {
                    return NotFound();
                }

                var genreForDelete = _genreRepository.Get(id);

                if (genreForDelete == null)
                {
                    return NotFound();
                }

                return View(genreForDelete);
            }
            catch
            {
                return RedirectToAction("AllGenres");
            }
        }

        [HttpDelete]
        public IActionResult DeleteGenre(int id)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound();
                }

                var deletedGenre = _genreRepository.Remove(id);

                if (deletedGenre == null)
                    return Json(new { success = false, message = "Unable to delete genre" });

                return Json(new { success = true, message = "Genre deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
