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
            // map BLGenre to Genre
            var dbGenre = _mapper.Map<Genre>(value);

            _dbContext.Genres.Add(dbGenre);
            _dbContext.SaveChanges();

            // map the new Genre back to BLGenre
            var blGenre = _mapper.Map<BLGenre>(dbGenre);

            return blGenre;
        }

        public BLGenre Modify(int id, BLGenre value)
        {
            // find the existing Genre entity in the database
            var dbGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);
            if (dbGenre == null)
                return null;

            // update the entity using the data from the BLGenre object
            _mapper.Map(value, dbGenre);

            _dbContext.Update(dbGenre);

            // save changes to the database
            _dbContext.SaveChanges();

            // map the updated Genre back to a BLGenre object
            var blGenre = _mapper.Map<BLGenre>(dbGenre);

            return blGenre;
        }

        public BLGenre Remove(int id)
        {
            // find the existing Genre entity in the database
            var dbGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);
            if (dbGenre == null)
                return null;

            // remove Genre from the database
            _dbContext.Genres.Remove(dbGenre);

            // save to the database
            _dbContext.SaveChanges();

            // map the removed Genre to a BLGenre object
            var blGenre = _mapper.Map<BLGenre>(dbGenre);

            return blGenre;
        }
    }
}
