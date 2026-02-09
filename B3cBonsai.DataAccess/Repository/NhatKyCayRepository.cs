using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using System.Linq;

namespace B3cBonsai.DataAccess.Repository
{
    public class NhatKyCayRepository : Repository<NhatKyCay>, INhatKyCayRepository
    {
        private ApplicationDbContext _db;
        public NhatKyCayRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(NhatKyCay obj)
        {
             _db.NhatKyCays.Update(obj);
        }
    }
}
