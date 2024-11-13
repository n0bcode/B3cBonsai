using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
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
        public NguoiDungUngDungRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(NguoiDungUngDung obj)
        {
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
                nguoiDungUngDung.LinkAnh = obj.LinkAnh;
                _db.NguoiDungUngDungs.Update(nguoiDungUngDung);
            }
        }
    }
}
