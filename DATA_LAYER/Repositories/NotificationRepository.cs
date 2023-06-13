using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA_LAYER.Repositories
{
    public interface INotificationRepository
    {
        IEnumerable<BLNotification> GetAll();
        BLNotification Get(int id);
        BLNotification Add(BLNotification value);
        BLNotification Modify(int id, BLNotification value);
        BLNotification Remove(int id);
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public NotificationRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext; 
        }

        public IEnumerable<BLNotification> GetAll()
        {
            var dbNotifications = _dbContext.Notifications;

            var blNotifications = _mapper.Map<IEnumerable<BLNotification>>(dbNotifications);

            return blNotifications;
        }

        public BLNotification Get(int id)
        {
            var dbNotifications = _dbContext.Notifications;

            var blNotifications = _mapper.Map<IEnumerable<BLNotification>>(dbNotifications);

            return blNotifications.FirstOrDefault(x => x.Id == id);
        }

        public BLNotification Add(BLNotification value)
        {
            var dbNotification = _mapper.Map<Notification>(value);

            _dbContext.Notifications.Add(dbNotification);
            _dbContext.SaveChanges();

            var blNotification = _mapper.Map<BLNotification>(dbNotification);

            return blNotification;
        }

        public BLNotification Modify(int id, BLNotification value)
        {
            var dbNotification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);
            if (dbNotification == null)
                return null;

            _mapper.Map(value, dbNotification);

            _dbContext.Update(dbNotification);

            _dbContext.SaveChanges();

            var blNotification = _mapper.Map<BLNotification>(dbNotification);

            return blNotification;
        }

        public BLNotification Remove(int id)
        {
            var dbNotification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);
            if (dbNotification == null)
                return null;

            _dbContext.Notifications.Remove(dbNotification);

            _dbContext.SaveChanges();

            var blNotification = _mapper.Map<BLNotification>(dbNotification);

            return blNotification;
        }


    }
}
