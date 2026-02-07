using System.Collections.Generic;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace B3cBonsai.Utility.Services
{
    public class CloudinaryFileStorageService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryFileStorageService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["CloudinarySettings:CloudName"],
                configuration["CloudinarySettings:ApiKey"],
                configuration["CloudinarySettings:ApiSecret"]);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> StoreFileAsync(IFormFile file, string subfolder, string rootFolder = "images")
        {
            var isImage = file.ContentType.StartsWith("image/");
            
            if (isImage)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = $"{rootFolder}/{subfolder}"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
            else
            {
                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = $"{rootFolder}/{subfolder}"
                };
                var uploadResult = await _cloudinary.UploadLargeAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
        }

        public async Task<IEnumerable<string>> StoreFilesAsync(IEnumerable<IFormFile> files, string subfolder, string rootFolder = "images")
        {
            var fileUrls = new List<string>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileUrl = await StoreFileAsync(file, subfolder, rootFolder);
                    fileUrls.Add(fileUrl);
                }
            }
            return fileUrls;
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return;
            }

            var publicId = GetPublicIdFromUrl(fileUrl);
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
