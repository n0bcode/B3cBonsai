
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace B3cBonsai.Utility.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LocalFileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> StoreFileAsync(IFormFile file, string subfolder, string rootFolder = "images")
        {
            var originalFileName = Path.GetFileName(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, rootFolder, subfolder, uniqueFileName);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/{rootFolder}/{subfolder}/{uniqueFileName}";
            return relativePath.Replace("\\", "/");
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
            if (string.IsNullOrEmpty(fileUrl)) return;

            // Handle both full URLs (if stored as such) and relative paths
            string relativePath = fileUrl;
            if (fileUrl.StartsWith("/") || fileUrl.StartsWith("http"))
            {
                try
                {
                    var uri = new System.Uri(fileUrl, UriKind.RelativeOrAbsolute);
                    relativePath = uri.IsAbsoluteUri ? uri.LocalPath : fileUrl;
                }
                catch { /* Fallback to raw string */ }
            }

            // Clean leading slashes for Path.Combine
            relativePath = relativePath.TrimStart('/');

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            await Task.CompletedTask;
        }
    }
}
