using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.DALModels;
using Microsoft.EntityFrameworkCore;
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
        (IEnumerable<BLVideo>, int) SearchVideos(string searchText, int? page, int? size);
        IEnumerable<BLVideo> SearchCardView(string searchText);
        int CountVideos();
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
            var dbVideos = _dbContext.Videos.Include("Genre");

            var blVideos = _mapper.Map<IEnumerable<BLVideo>>(dbVideos);

            return blVideos;
        }

        public BLVideo Get(int id)
        {
            var dbVideos = _dbContext.Videos.Include("Genre");

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

        public (IEnumerable<BLVideo>, int) SearchVideos(string searchText, int? page, int? size)
        {
            var videos = _dbContext.Videos
                .Include("Genre");


            if (searchText != null)
            {
                searchText = searchText.ToLower();

                videos = videos.Where(x =>
                    x.Name.ToLower().Contains(searchText) ||
                    x.Genre.Name.ToLower().Contains(searchText));
            }

            var unpagedCount = videos.Count();

            if (page != null && size != null)
            {
                videos = videos.Skip(page.Value * size.Value).Take(size.Value);
            }

            return (_mapper.Map<IEnumerable<BLVideo>>(videos), unpagedCount);
        }

        public int CountVideos() 
           => _dbContext.Videos.Count();


        public IEnumerable<BLVideo> SearchCardView(string searchText)
        {
            var videos = _dbContext.Videos
                .Include("Genre");

            if (searchText != null)
            {
                videos = videos.Where(x => x.Name.Contains(searchText));
            }

            return _mapper.Map<IEnumerable<BLVideo>>(videos);
        }
    }
}
