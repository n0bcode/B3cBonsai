
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace B3cBonsai.Utility.Services
{
    /// <summary>
    /// Generic interface for file storage operations (images, 3D models, etc.)
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Stores a single file in the specified root and subfolder.
        /// </summary>
        Task<string> StoreFileAsync(IFormFile file, string subfolder, string rootFolder = "images");

        /// <summary>
        /// Deletes a file given its URL or relative path.
        /// </summary>
        Task DeleteFileAsync(string fileUrl);

        /// <summary>
        /// Stores multiple files in the specified root and subfolder.
        /// </summary>
        Task<IEnumerable<string>> StoreFilesAsync(IEnumerable<IFormFile> files, string subfolder, string rootFolder = "images");
    }
}
