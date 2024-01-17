using DATA_LAYER.BLModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
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
        public IActionResult CreateTag(BLTag blTag)
        {
            try
            {
                if (!ModelState.IsValid) return View(blTag);

                _tagRepository.Add(blTag);

                return RedirectToAction("AllTags");
            }
            catch 
            {
                return RedirectToAction("AllTags");
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
            catch 
            {
                return RedirectToAction("AllTags");
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
            catch 
            {
                return RedirectToAction("AllTags");
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
            catch 
            {
                return RedirectToAction("AllTags");
            }
        }

    }
}
