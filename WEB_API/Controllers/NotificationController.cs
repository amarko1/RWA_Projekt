using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WEB_API.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            var notifications = _notificationRepository.GetAll();
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var notification = _notificationRepository.Get(id);
            if (notification == null)
                return NotFound();
            return Ok(notification);
        }

        [HttpPost]
        public ActionResult Add([FromBody] BLNotification notification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var addedNotification = _notificationRepository.Add(notification);

            return CreatedAtAction(nameof(Get), new { id = addedNotification.Id }, addedNotification);
        }

        [HttpPut("{id}")]
        public ActionResult Modify(int id, [FromBody] BLNotification notification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedNotification = _notificationRepository.Modify(id, notification);
            if (updatedNotification == null)
                return NotFound();
            return Ok(updatedNotification);
        }

        [HttpDelete("{id}")]
        public ActionResult Remove(int id)
        {
            var removedNotification = _notificationRepository.Remove(id);
            if (removedNotification == null)
                return NotFound();
            return Ok(removedNotification);
        }
    }
}
