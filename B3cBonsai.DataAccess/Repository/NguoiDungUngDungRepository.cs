using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.DataAccess.Repository
{
    public class NguoiDungUngDungRepository : Repository<NguoiDungUngDung>, INguoiDungUngDungRepository
    {
        private ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public NguoiDungUngDungRepository(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment) : base(db)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
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
                    string wwwRootPath = _webHostEnvironment.WebRootPath;

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string nguoiDungUngDungPath = Path.Combine("images", "user", "user-" + obj.Id);
                    string finalPath = Path.Combine(wwwRootPath, nguoiDungUngDungPath);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(nguoiDungUngDung.LinkAnh))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, nguoiDungUngDung.LinkAnh.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    // Save the new image
                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Update the LinkAnh property with the new image path
                    nguoiDungUngDung.LinkAnh = $"\\{Path.Combine(nguoiDungUngDungPath, fileName).Replace("\\", "/")}";
                }

                // Update the user details in database
                _db.NguoiDungUngDungs.Update(nguoiDungUngDung);
            }
        }


    }
}
