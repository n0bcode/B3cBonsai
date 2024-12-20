﻿using B3cBonsai.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.DataAccess.Repository.IRepository
{
    public interface INguoiDungUngDungRepository : IRepository<NguoiDungUngDung>
    {
        public Task UpdateUserInfoAndImage(NguoiDungUngDung obj, IFormFile? file);
    }
}
