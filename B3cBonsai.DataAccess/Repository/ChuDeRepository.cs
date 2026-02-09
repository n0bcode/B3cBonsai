using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace B3cBonsai.DataAccess.Repository
{
    public class ChuDeRepository : Repository<ChuDe>, IChuDeRepository
    {
        private ApplicationDbContext _db;
        public ChuDeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ChuDe obj)
        {
             _db.ChuDes.Update(obj);
        }

        public void IncrementViewCount(int id)
        {
            _db.ChuDes.Where(x => x.Id == id).ExecuteUpdate(x => x.SetProperty(p => p.LuotXem, p => p.LuotXem + 1));
        }
    }
}
