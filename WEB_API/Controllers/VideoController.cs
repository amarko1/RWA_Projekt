using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WEB_API.Controllers
{
    [Route("api/videos")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IMapper _mapper;

        public VideoController(IVideoRepository videoRepository, IMapper mapper)
        {
            _videoRepository = videoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            var videos = _videoRepository.GetAll();
            return Ok(videos);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var video = _videoRepository.Get(id);
            if (video == null)
                return NotFound();
            return Ok(video);
        }

        [HttpPost]
        public ActionResult Add([FromBody] BLVideo video)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var addedVideo = _videoRepository.Add(video);

            return CreatedAtAction(nameof(Get), new { id = addedVideo.Id }, addedVideo);
        }

        [HttpPut("{id}")]
        public ActionResult Modify(int id, [FromBody] BLVideo video)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedVideo = _videoRepository.Modify(id, video);
            if (updatedVideo == null)
                return NotFound();
            return Ok(updatedVideo);
        }

        [HttpDelete("{id}")]
        public ActionResult Remove(int id)
        {
            var removedVideo = _videoRepository.Remove(id);
            if (removedVideo == null)
                return NotFound();
            return Ok(removedVideo);
        }

        [HttpGet("[action]")]
        public ActionResult<List<BLVideo>> FilterByName(string name)
        {
            try
            {
                var filteredVideos = _videoRepository.GetAll()
                    .Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return filteredVideos;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<BLVideo> SortingAndPagging(int page, int size, string orderBy, string direction)
        {
            var videos = _videoRepository.GetAll();

            if (string.Compare(orderBy, "id", true) == 0)
            {
                videos.OrderBy(x => x.Id);
            }
            else if (string.Compare(orderBy, "name", true) == 0)
            {
                videos.OrderBy(x => x.Name);
            }
            else if (string.Compare(orderBy, "totalseconds", true) == 0)
            {
                videos.OrderBy(x => x.TotalSeconds);
            }
            else
            {
                videos.OrderBy(x => x.Id);
            }

            // For descending order we just reverse it
            if (string.Compare(direction, "desc", true) == 0)
            {
                videos.Reverse();
            }

            // Now we can page the correctly ordered items
            var retVal = videos.Skip(page * size).Take(size).ToList();

            return Ok(retVal);
        }
    }
}
