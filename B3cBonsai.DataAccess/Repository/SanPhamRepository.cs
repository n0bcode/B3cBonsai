﻿using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.DataAccess.Repository
{
    public class SanPhamRepository : Repository<SanPham>, ISanPhamRepository
    {
        private ApplicationDbContext _db;
        public SanPhamRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(SanPham obj)
        {
            _db.SanPhams.Update(obj);
        }
    }
}
