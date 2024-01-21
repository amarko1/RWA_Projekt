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
    public interface ICountryRepository
    {
        IEnumerable<BLCountry> GetAll();
        BLCountry Get(int id);
        BLCountry Add(BLCountry value);
        BLCountry Modify(int id, BLCountry value);
        BLCountry Remove(int id);
        (IEnumerable<BLCountry>, int) SearchCountries(string searchText, int? page, int? size);
        int CountCountries();
    }

    public class CountryRepository : ICountryRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public CountryRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<BLCountry> GetAll()
        {
            var dbCountries = _dbContext.Countries;

            var blCountries = _mapper.Map<IEnumerable<BLCountry>>(dbCountries);

            return blCountries;
        }

        public BLCountry Get(int id)
        {
            var dbCountries = _dbContext.Countries;

            var blCountries = _mapper.Map<IEnumerable<BLCountry>>(dbCountries);

            return blCountries.FirstOrDefault(x => x.Id == id);
        }

        public BLCountry Add(BLCountry value)
        {
            var dbCountries = _mapper.Map<Country>(value);

            _dbContext.Countries.Add(dbCountries);
            _dbContext.SaveChanges();

            var blCountries = _mapper.Map<BLCountry>(dbCountries);

            return blCountries;
        }

        public BLCountry Modify(int id, BLCountry value)
        {
            var dbCountries = _dbContext.Countries.FirstOrDefault(x => x.Id == id);
            if (dbCountries == null)
                return null;

            _mapper.Map(value, dbCountries);

            _dbContext.Update(dbCountries);

            _dbContext.SaveChanges();

            var blCountries = _mapper.Map<BLCountry>(dbCountries);

            return blCountries;
        }

        public BLCountry Remove(int id)
        {
            var dbCountries = _dbContext.Countries.FirstOrDefault(x => x.Id == id);
            if (dbCountries == null)
                return null;

            _dbContext.Countries.Remove(dbCountries);

            _dbContext.SaveChanges();

            var blCountries = _mapper.Map<BLCountry>(dbCountries);

            return blCountries;
        }

        public (IEnumerable<BLCountry>, int) SearchCountries(string searchText, int? page, int? size)
        {
            var countries = _dbContext.Countries
                .Include("Users");

            if (searchText != null)
            {
                countries = countries.Where(x => x.Name.Contains(searchText));
            }
            var unpagedCount = countries.Count();

            if (page != null && size != null)
            {
                countries = countries.Skip(page.Value * size.Value).Take(size.Value);
            }

            return (_mapper.Map<IEnumerable<BLCountry>>(countries), unpagedCount);
        }

        public int CountCountries()
           => _dbContext.Countries.Count();
    }
}
