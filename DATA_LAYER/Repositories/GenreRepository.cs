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

    public interface IGenreRepository
    {
        IEnumerable<BLGenre> GetAll();
        BLGenre Get(int id);
        BLGenre Add(BLGenre value);
        BLGenre Modify(int id, BLGenre value);
        BLGenre Remove(int id);
    }

    public class GenreRepository : IGenreRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public GenreRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<BLGenre> GetAll()
        {
            var dbGenres = _dbContext.Genres;

            var blGenres = _mapper.Map<IEnumerable<BLGenre>>(dbGenres);

            return blGenres;
        }

        public BLGenre Get(int id)
        {
            var dbGenres = _dbContext.Genres;

            var blGenres = _mapper.Map<IEnumerable<BLGenre>>(dbGenres);

            return blGenres.FirstOrDefault(x => x.Id == id);
        }

        public BLGenre Add(BLGenre value)
        {
            var dbGenre = _mapper.Map<Genre>(value);

            _dbContext.Genres.Add(dbGenre);
            _dbContext.SaveChanges();

            var blGenre = _mapper.Map<BLGenre>(dbGenre);

            return blGenre;
        }

        public BLGenre Modify(int id, BLGenre value)
        {
            var dbGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);
            if (dbGenre == null)
                return null;

            _mapper.Map(value, dbGenre);

            _dbContext.Update(dbGenre);

            _dbContext.SaveChanges();

            var blGenre = _mapper.Map<BLGenre>(dbGenre);

            return blGenre;
        }

        public BLGenre Remove(int id)
        {
            var dbGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);
            if (dbGenre == null)
                return null;

            _dbContext.Genres.Remove(dbGenre);

            _dbContext.SaveChanges();

            var blGenre = _mapper.Map<BLGenre>(dbGenre);

            return blGenre;
        }
    }
}
