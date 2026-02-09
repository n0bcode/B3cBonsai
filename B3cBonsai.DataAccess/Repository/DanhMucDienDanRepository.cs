using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using System.Linq;

namespace B3cBonsai.DataAccess.Repository
{
    public class DanhMucDienDanRepository : Repository<DanhMucDienDan>, IDanhMucDienDanRepository
    {
        private ApplicationDbContext _db;
        public DanhMucDienDanRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(DanhMucDienDan obj)
        {
             _db.DanhMucDienDans.Update(obj);
        }
    }
}
