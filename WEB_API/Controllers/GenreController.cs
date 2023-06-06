using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WEB_API.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;

        public GenreController(IGenreRepository genreRepository, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            var genres = _genreRepository.GetAll();
            return Ok(genres);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var genre = _genreRepository.Get(id);
            if (genre == null)
                return NotFound();
            return Ok(genre);
        }

        [HttpPost]
        public ActionResult Add([FromBody] BLGenre genre)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var addedGenre = _genreRepository.Add(genre);

            return CreatedAtAction(nameof(Get), new { id = addedGenre.Id }, addedGenre);
        }

        [HttpPut("{id}")]
        public ActionResult Modify(int id, [FromBody] BLGenre genre)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedGenre = _genreRepository.Modify(id, genre);
            if (updatedGenre == null)
                return NotFound();
            return Ok(updatedGenre);
        }

        [HttpDelete("{id}")]
        public ActionResult Remove(int id)
        {
            var removedGenre = _genreRepository.Remove(id);
            if (removedGenre == null)
                return NotFound();
            return Ok(removedGenre);
        }

        //[HttpGet("[action]")]
        //public ActionResult<List<BLGenre>> FilterByName(string name)
        //{
        //    try
        //    {
        //        var filteredGenres = _genreRepository.GetAll()
        //            .Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
        //            .ToList();

        //        return filteredGenres;
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}

        //[HttpGet("[action]")]
        //public ActionResult<BLGenre> SortingAndPagging(int page, int size, string orderBy, string direction)
        //{
        //    var genres = _genreRepository.GetAll();

        //    // Ordering
        //    if (string.Compare(orderBy, "id", true) == 0)
        //    {
        //        genres.OrderBy(x => x.Id);
        //    }
        //    else if (string.Compare(orderBy, "name", true) == 0)
        //    {
        //        genres.OrderBy(x => x.Name);
        //    }
        //    else
        //    {
        //        // default: order by Id
        //        genres.OrderBy(x => x.Id);
        //    }

        //    // For descending order we just reverse it
        //    if (string.Compare(direction, "desc", true) == 0)
        //    {
        //        genres.Reverse();
        //    }

        //    // Now we can page the correctly ordered items
        //    var retVal = genres.Skip(page * size).Take(size).ToList();

        //    return Ok(retVal);
        //}
    }
}
