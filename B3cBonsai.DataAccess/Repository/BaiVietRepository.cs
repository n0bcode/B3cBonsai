using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace B3cBonsai.DataAccess.Repository
{
    public class BaiVietRepository : Repository<BaiViet>, IBaiVietRepository
    {
        private ApplicationDbContext _db;
        public BaiVietRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(BaiViet obj)
        {
             _db.BaiViets.Update(obj);
        }

        public void DeleteByThreadId(int threadId)
        {
            _db.BaiViets.Where(x => x.ChuDeId == threadId).ExecuteDelete();
        }
    }
}
