using System.Threading.Tasks;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility.Services;
using Microsoft.AspNetCore.Http;

namespace B3cBonsai.DataAccess.Repository
{
    public class NguoiDungUngDungRepository : Repository<NguoiDungUngDung>, INguoiDungUngDungRepository
    {
        private ApplicationDbContext _db;
        private readonly IImageStorageService _imageStorageService;
        public NguoiDungUngDungRepository(ApplicationDbContext db, IImageStorageService imageStorageService) : base(db)
        {
            _db = db;
            _imageStorageService = imageStorageService;
        }
        public async Task UpdateUserInfoAndImage(NguoiDungUngDung obj, IFormFile? file = null)
        {
            // Update the user information
            NguoiDungUngDung? nguoiDungUngDung = _db.NguoiDungUngDungs.FirstOrDefault(nd => nd.Id == obj.Id);
            if (nguoiDungUngDung != null)
            {
                nguoiDungUngDung.HoTen = obj.HoTen;
                nguoiDungUngDung.NgaySinh = obj.NgaySinh;
                nguoiDungUngDung.SoDienThoai = obj.SoDienThoai;
                nguoiDungUngDung.GioiTinh = obj.GioiTinh;
                nguoiDungUngDung.CCCD = obj.CCCD;
                nguoiDungUngDung.DiaChi = obj.DiaChi;
                nguoiDungUngDung.MoTa = obj.MoTa;
                nguoiDungUngDung.Email = obj.Email;

                // Handle image upload if there's a file provided
                if (file != null)
                {
                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(nguoiDungUngDung.LinkAnh))
                    {
                        await _imageStorageService.DeleteImageAsync(nguoiDungUngDung.LinkAnh);
                    }

                    // Save the new image
                    string subfolder = $"user/user-{obj.Id}";
                    nguoiDungUngDung.LinkAnh = await _imageStorageService.StoreImageAsync(file, subfolder);
                }

                // Update the user details in database
                _db.NguoiDungUngDungs.Update(nguoiDungUngDung);
            }
        }


    }
}
