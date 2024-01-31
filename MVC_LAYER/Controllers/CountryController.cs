using DATA_LAYER.BLModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_LAYER.Controllers
{
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;

        public CountryController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public IActionResult SearchCountry(string searchText, bool search, int? dataPage, int? dataSize = 10)
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

            (var allCountries, var unpagedCount) = _countryRepository.SearchCountries(searchText, dataPage, dataSize);

            ViewBag.TotalPages = unpagedCount / dataSize;


            return View(allCountries);
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

        public IActionResult AllCountries()
        {
            var allCountries = _countryRepository.GetAll();

            return View(allCountries);
        }

        public IActionResult CreateCountry()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCountry(BLCountry blCountry)
        {
            try
            {
                if (!ModelState.IsValid) return View(blCountry);

                _countryRepository.Add(blCountry);

                return RedirectToAction("AllCountries");
            }
            catch
            {
                return RedirectToAction("AllCountries");
            }
        }


        public IActionResult EditCountry(int id)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound();
                }

                var countryForEdit = _countryRepository.Get(id);

                if (countryForEdit == null)
                {
                    return NotFound();
                }

                return View(countryForEdit);
            }
            catch
            {
                return RedirectToAction("AllCountries");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCountry(BLCountry country)
        {
            try
            {
                if (!ModelState.IsValid) return View(country);

                int id = country.Id;

                _countryRepository.Modify(id, country);

                return RedirectToAction("AllCountries");
            }
            catch
            {
                return RedirectToAction("AllCountries");
            }
        }

        public IActionResult DeleteCountry(int id, BLCountry country) //fix
        {
            try
            {
                if (id == 0)
                {
                    return NotFound();
                }

                var countryForDelete = _countryRepository.Get(id);

                if (countryForDelete == null)
                {
                    return NotFound();
                }

                return View(countryForDelete);
            }
            catch
            {
                return RedirectToAction("AllCountries");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCountry(int id)
        {
            try
            {
                if (id == 0) return NotFound();

                var deletedCountry = _countryRepository.Remove(id);

                if (deletedCountry == null)
                    return Json(new { success = false, message = "Unable to delete country" });

                return RedirectToAction("AllCountries");
            }
            catch
            {
                return RedirectToAction("AllCountries");
            }
        }
    }
}
