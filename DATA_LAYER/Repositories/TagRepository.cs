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
    public interface ITagRepository
    {
        IEnumerable<BLTag> GetAll();
        BLTag Get(int id);
        BLTag Add(BLTag value);
        BLTag Modify(int id, BLTag value);
        BLTag Remove(int id);
    }

    public class TagRepository : ITagRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public TagRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<BLTag> GetAll()
        {
            var dbTags = _dbContext.Tags;

            var blTags = _mapper.Map<IEnumerable<BLTag>>(dbTags);

            return blTags;
        }

        public BLTag Get(int id)
        {
            var dbTags = _dbContext.Tags;

            var blTags = _mapper.Map<IEnumerable<BLTag>>(dbTags);

            return blTags.FirstOrDefault(x => x.Id == id);
        }

        public BLTag Add(BLTag value)
        {
            var dbTag = _mapper.Map<Tag>(value);

            _dbContext.Tags.Add(dbTag);
            _dbContext.SaveChanges();

            var blTag = _mapper.Map<BLTag>(dbTag);

            return blTag;
        }

        public BLTag Modify(int id, BLTag value)
        {
            var dbTag = _dbContext.Tags.FirstOrDefault(x => x.Id == id);
            if (dbTag == null)
                return null;

            _mapper.Map(value, dbTag);

            _dbContext.Update(dbTag);

            _dbContext.SaveChanges();

            var blTag = _mapper.Map<BLTag>(dbTag);

            return blTag;
        }

        public BLTag Remove(int id)
        {
            var dbTag = _dbContext.Tags.FirstOrDefault(x => x.Id == id);
            if (dbTag == null)
                return null;

            _dbContext.Tags.Remove(dbTag);

            _dbContext.SaveChanges();

            var blTag = _mapper.Map<BLTag>(dbTag);

            return blTag;
        }

    }
}
