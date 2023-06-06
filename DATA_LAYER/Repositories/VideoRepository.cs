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
    public interface IVideoRepository
    {
        IEnumerable<BLVideo> GetAll();
        BLVideo Get(int id);
        BLVideo Add(BLVideo value);
        BLVideo Modify(int id, BLVideo value);
        BLVideo Remove(int id);
    }

    public class VideoRepository : IVideoRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public VideoRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
           _dbContext = dbContext;
           _mapper = mapper;
        }

        public IEnumerable<BLVideo> GetAll()
        {
            var dbVideos = _dbContext.Videos;

            var blVideos = _mapper.Map<IEnumerable<BLVideo>>(dbVideos);

            return blVideos;
        }

        public BLVideo Get(int id)
        {
            var dbVideos = _dbContext.Videos;

            var blVideos = _mapper.Map<IEnumerable<BLVideo>>(dbVideos);

            return blVideos.FirstOrDefault(x => x.Id == id);
        }

        public BLVideo Add(BLVideo value)
        {
            // map BLVideo to Video
            var dbVideo = _mapper.Map<Video>(value);

            _dbContext.Videos.Add(dbVideo);
            _dbContext.SaveChanges();

            // map the new Video back to BLVideo
            var blVideo = _mapper.Map<BLVideo>(dbVideo);

            return blVideo;
        }

        public BLVideo Modify(int id, BLVideo value)
        {
            var dbVideo = _dbContext.Videos.FirstOrDefault(x => x.Id == id);
            if (dbVideo == null)
                return null;

            _mapper.Map(value, dbVideo);

            _dbContext.Update(dbVideo);

            _dbContext.SaveChanges();

            var blVideo = _mapper.Map<BLVideo>(dbVideo);

            return blVideo;
        }

        public BLVideo Remove(int id)
        {
            // find the existing Video entity in the database
            var dbVideo = _dbContext.Videos.FirstOrDefault(x => x.Id == id);
            if (dbVideo == null)
                return null;

            // remove the Video entity from the database
            _dbContext.Videos.Remove(dbVideo);

            // save changes to the database
            _dbContext.SaveChanges();

            // map the removed Video to a BLVideo object
            var blVideo = _mapper.Map<BLVideo>(dbVideo);

            return blVideo;
        }
    }
}
