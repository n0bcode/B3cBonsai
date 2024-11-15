using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.Models.ViewModels
{
    public class SanPhamVM
    {
        public SanPham SanPham { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> DanhMucSanPham { get; set; }
    }
}
