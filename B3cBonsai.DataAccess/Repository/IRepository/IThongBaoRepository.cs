using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;

namespace B3cBonsai.DataAccess.Repository.IRepository
{
    public interface IThongBaoRepository : IRepository<ThongBao>
    {
        void Update(ThongBao thongBao);
    }
}
