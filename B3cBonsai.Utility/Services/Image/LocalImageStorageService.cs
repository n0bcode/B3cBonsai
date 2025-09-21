
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace B3cBonsai.Utility.Services
{
    public class LocalImageStorageService : IImageStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LocalImageStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> StoreImageAsync(IFormFile file, string subfolder)
        {
            var originalFileName = Path.GetFileName(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", subfolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/{subfolder}/{uniqueFileName}";
        }

        public async Task<IEnumerable<string>> StoreImagesAsync(IEnumerable<IFormFile> files, string subfolder)
        {
            var imageUrls = new List<string>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var imageUrl = await StoreImageAsync(file, subfolder);
                    imageUrls.Add(imageUrl);
                }
            }
            return imageUrls;
        }

        public Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return Task.CompletedTask;
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}
