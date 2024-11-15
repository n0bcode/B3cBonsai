using B3cBonsai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.DataAccess.Repository.IRepository
{
    public interface IDanhMucSanPhamRepository : IRepository<DanhMucSanPham>
    {
        public void Update(DanhMucSanPham obj);

        // Thêm phương thức GetFirstOrDefault để lấy một bản ghi theo điều kiện
        DanhMucSanPham GetFirstOrDefault(Expression<Func<DanhMucSanPham, bool>> filter);
    }
}
