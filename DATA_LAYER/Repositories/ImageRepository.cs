using AutoMapper;
using DATA_LAYER.BLModels;
using DATA_LAYER.DALModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA_LAYER.Repositories
{
    public interface IImageService
    {
        BLImage? Get(int id);
        BLImage Create(IFormFile image);

        BLImage? Update(int id, IFormFile image);
    }

    public class ImageService : IImageService
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public ImageService(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private byte[]? GetFileAsMemoryStream(IFormFile file)
        {
            if (file != null)
            {
                if (file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);

                        if (memoryStream.Length < 50 * 1024 * 1024)
                        {
                            return memoryStream.ToArray();
                        }
                    }
                }
            }

            return null;
        }

        public BLImage Create(IFormFile image)
        {
            var imageAsStream = GetFileAsMemoryStream(image);

            var imageForMap = _mapper.Map<Image>(image);

            Image newImage = new Image();

            if (imageAsStream != null)
            {
                newImage.Content = Convert.ToBase64String(imageAsStream);
            }

            _dbContext.Images.Add(imageForMap);
            _dbContext.SaveChanges();

            var blImages = _mapper.Map<BLImage>(imageForMap);

            return blImages;
        }

        public BLImage? Get(int id)
        {
            var requestedImage = _dbContext.Images;

            if (requestedImage == null) return null;

            var blImages = _mapper.Map<IEnumerable<BLImage>>(requestedImage);

            return blImages.FirstOrDefault(x => x.Id == id);
        }

        public BLImage? Update(int id, IFormFile image)
        {
            var imageForUpdate = _dbContext.Images.FirstOrDefault(i => i.Id == id);

            if (imageForUpdate == null) return null;

            var imageAsStream = GetFileAsMemoryStream(image);

            if (imageAsStream != null)
            {
                imageForUpdate.Content = Convert.ToBase64String(imageAsStream);
            }

            //_mapper.Map(image, imageForUpdate);

            _dbContext.Update(imageForUpdate);

            _dbContext.SaveChanges();

            var blCountries = _mapper.Map<BLImage>(imageForUpdate);

            return blCountries;
        }
    }
}
