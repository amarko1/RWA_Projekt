using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WEB_API.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagController(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            var tags = _tagRepository.GetAll();
            return Ok(tags);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var tag = _tagRepository.Get(id);
            if (tag == null)
                return NotFound();
            return Ok(tag);
        }

        [HttpPost]
        public ActionResult Add([FromBody] BLTag tag)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var addedTag = _tagRepository.Add(tag);

            return CreatedAtAction(nameof(Get), new { id = addedTag.Id }, addedTag);
        }

        [HttpPut("{id}")]
        public ActionResult Modify(int id, [FromBody] BLTag tag)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedTag = _tagRepository.Modify(id, tag);
            if (updatedTag == null)
                return NotFound();
            return Ok(updatedTag);
        }

        [HttpDelete("{id}")]
        public ActionResult Remove(int id)
        {
            var removedTag = _tagRepository.Remove(id);
            if (removedTag == null)
                return NotFound();
            return Ok(removedTag);
        }
    }
}
