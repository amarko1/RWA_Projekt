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
    public interface IImageRepository
    {
        Task<int> AddImageAsync(BLImage image);
        Task UpdateImageAsync(int imageId, string newImagePath);
        Task SaveChangesAsync();
        string GetImagePathById(int imageId);
    }

    public class ImageRepository : IImageRepository
    {
        private readonly RwaMoviesContext _context; 

        public ImageRepository(RwaMoviesContext context)
        {
            _context = context;
        }

        public async Task<int> AddImageAsync(BLImage blImage)
        {
            var image = new Image
            {
                Content = blImage.Content
            };

            _context.Images.Add(image);
            await _context.SaveChangesAsync();
            return image.Id; 
        }

        public async Task UpdateImageAsync(int imageId, string newImagePath)
        {
            var image = await _context.Images.FindAsync(imageId);
            if (image != null)
            {
                image.Content = newImagePath;
                _context.Update(image);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public string GetImagePathById(int imageId)
        {
            var image = _context.Images.FirstOrDefault(img => img.Id == imageId);
            return image?.Content; 
        }
    }
}
