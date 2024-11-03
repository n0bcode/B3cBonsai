﻿using B3cBonsai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.DataAccess.Repository.IRepository
{
    public interface IDanhMucSanPhamRepository : IRepository<DanhMucSanPham>
    {
        public void Update(DanhMucSanPham obj);
    }
}
