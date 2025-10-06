using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;

namespace B3cBonsai.DataAccess.Repository
{
    public class ThongBaoRepository : Repository<ThongBao>, IThongBaoRepository
    {
        private ApplicationDbContext _db;
        public ThongBaoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ThongBao thongBao)
        {
            _db.ThongBaos.Update(thongBao);
        }
    }
}
