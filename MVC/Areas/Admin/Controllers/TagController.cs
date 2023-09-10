using DATA_LAYER.BLModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly ITagRepository _tagRepository;

        public TagController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public IActionResult AllTags()
        {
            var allTags = _tagRepository.GetAll();

            return View(allTags);
        }

        public IActionResult CreateTag()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTag ([FromBody] BLTag blTag)
        {
            try
            {
                if (!ModelState.IsValid) return View(blTag);

                _tagRepository.Add(blTag);

                return RedirectToAction("AllTags");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }


        public IActionResult EditTag(int id)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound();
                }

                var tagForEdit = _tagRepository.Get(id);

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
        public IActionResult EditTag(BLTag tag)
        {
            try
            {
                if (!ModelState.IsValid) return View(tag);

                int id = tag.Id;

                _tagRepository.Modify(id, tag);

                return RedirectToAction("AllTags");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }

        [HttpDelete]
        public IActionResult DeleteTag(int id)
        {
            try
            {
                if (id == 0) return NotFound();

                var deletedTag = _tagRepository.Remove(id);

                if (deletedTag == null)
                    return Json(new { success = false, message = "Unable to delete tag" });

                return Json(new { success = true, message = "Tag deleted successfully" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }



    }
}
