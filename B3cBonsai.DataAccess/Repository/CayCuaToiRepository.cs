using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using System.Linq;

namespace B3cBonsai.DataAccess.Repository
{
    public class CayCuaToiRepository : Repository<CayCuaToi>, ICayCuaToiRepository
    {
        private ApplicationDbContext _db;
        public CayCuaToiRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CayCuaToi obj)
        {
             _db.CayCuaTois.Update(obj);
        }
    }
}
