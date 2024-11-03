using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
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
            _db.DonHangs.Update(obj);
        }
    }
}
