
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace B3cBonsai.Utility.Services
{
    public interface IImageStorageService
    {
        Task<string> StoreImageAsync(IFormFile file, string subfolder);
        Task DeleteImageAsync(string imageUrl);
        Task<IEnumerable<string>> StoreImagesAsync(IEnumerable<IFormFile> files, string subfolder);
    }
}
