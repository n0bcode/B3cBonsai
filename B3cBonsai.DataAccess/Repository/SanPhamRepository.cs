using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.DataAccess.Repository
{
    public class SanPhamRepository : Repository<SanPham>, ISanPhamRepository
    {
        private ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SanPhamRepository(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment) : base(db)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public void Update(SanPham obj)
        {
            _db.SanPhams.Update(obj);
        }

        public void UpdateWithFiles(SanPham obj, IEnumerable<IFormFile>? images, IEnumerable<IFormFile>? videos)
        {
            SanPham sanPham = _db.SanPhams.FirstOrDefault(sp => sp.Id == obj.Id);
            if (sanPham != null) {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                // Nếu người dùng không thêm hình ảnh mới, không cần xóa hình ảnh cũ
                if (images != null)
                {

                    foreach (IFormFile file in images)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + sanPham.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        HinhAnhSanPham productImage = new()
                        {
                            LinkAnh = @"\" + productPath + @"\" + fileName,
                            SanPhamId = sanPham.Id,
                        };

                        if (sanPham.HinhAnhs == null)
                            sanPham.HinhAnhs = new List<HinhAnhSanPham>();

                        sanPham.HinhAnhs.Add(productImage);

                    }

                    _db.SanPhams.Update(sanPham);
                }

            }
        }
    }
}
