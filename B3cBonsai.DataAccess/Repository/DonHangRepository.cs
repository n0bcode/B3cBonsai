using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.DataAccess.Repository
{
    public class DonHangRepository : Repository<DonHang>, IDonHangRepository
    {
        private ApplicationDbContext _db;
        public DonHangRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(DonHang obj)
        {
            if (obj.TrangThaiDonHang == SD.StatusCancelled)
            {
                ReturnObject(obj.Id);
            }
            _db.DonHangs.Update(obj);
        }
        public void ReturnObject(int id)
        {
            // Lấy chi tiết đơn hàng dựa trên id
            var chiTietDonHangs = _db.ChiTietDonHangs.Where(d => d.DonHangId == id).ToList();

            // Duyệt qua chi tiết đơn hàng để cộng lại số lượng sản phẩm và combo
            foreach (var chiTiet in chiTietDonHangs)
            {
                if (chiTiet.LoaiDoiTuong == SD.ObjectDetailOrder_SanPham)
                {
                    // Nếu là sản phẩm, cộng lại số lượng sản phẩm trong kho
                    var sanPham = _db.SanPhams.FirstOrDefault(sp => sp.Id == chiTiet.SanPhamId);
                    if (sanPham != null)
                    {
                        sanPham.SoLuong += chiTiet.SoLuong; // Cộng số lượng sản phẩm
                    }
                }
                else if (chiTiet.LoaiDoiTuong == SD.ObjectDetailOrder_Combo)
                {
                    // Nếu là combo, cộng lại số lượng combo trong kho
                    var combo = _db.ComboSanPhams.FirstOrDefault(c => c.Id == chiTiet.ComboId);
                    if (combo != null)
                    {
                        combo.SoLuong += chiTiet.SoLuong; // Cộng số lượng combo
                    }
                }
            }

            // Lưu thay đổi trong cơ sở dữ liệu
            _db.SaveChanges();
        }
    }
}
