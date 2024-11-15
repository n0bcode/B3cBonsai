using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.DataAccess.Repository
{
    public class DanhMucSanPhamRepository : Repository<DanhMucSanPham>, IDanhMucSanPhamRepository
    {
        private ApplicationDbContext _db;
        public DanhMucSanPhamRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(DanhMucSanPham obj)
        {
            _db.DanhMucSanPhams.Update(obj);
        }

        
        public DanhMucSanPham GetFirstOrDefault(Expression<Func<DanhMucSanPham, bool>> filter)
        {
            return _db.DanhMucSanPhams.FirstOrDefault(filter);
        }
    }
}
