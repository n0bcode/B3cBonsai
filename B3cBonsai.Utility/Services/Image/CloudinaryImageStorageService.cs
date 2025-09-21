using System.Collections.Generic;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace B3cBonsai.Utility.Services
{
    public class CloudinaryImageStorageService : IImageStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageStorageService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["CloudinarySettings:CloudName"],
                configuration["CloudinarySettings:ApiKey"],
                configuration["CloudinarySettings:ApiSecret"]);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> StoreImageAsync(IFormFile file, string subfolder)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = subfolder
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
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

        public async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return;
            }

            var publicId = GetPublicIdFromUrl(imageUrl);
            if (string.IsNullOrEmpty(publicId))
            {
                return;
            }

            var deletionParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deletionParams);
        }

        private string GetPublicIdFromUrl(string url)
        {
            try
            {
                var uri = new System.Uri(url);
                var parts = uri.Segments;
                var publicIdWithExtension = string.Join("", parts, 4, parts.Length - 4);
                return publicIdWithExtension.Substring(0, publicIdWithExtension.LastIndexOf('.'));
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
