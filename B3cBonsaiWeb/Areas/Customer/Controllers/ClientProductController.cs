using Microsoft.AspNetCore.Mvc;
using B3cBonsai.Models;
using Microsoft.EntityFrameworkCore;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using X.PagedList;
using X.PagedList.Extensions;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ClientProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientProductController(IUnitOfWork context)
        {
            _unitOfWork = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ListPagedProduct(int? page, string? findText, int[]? selectedCategories, int? minPrice, int? maxPrice, bool? inStock, string? SortBy)
        {
            TempData["findText"] = findText ?? "";
            TempData["selectedCategories"] = selectedCategories;
            TempData["minPrice"] = minPrice ?? 0;
            TempData["maxPrice"] = maxPrice ?? 0;
            TempData["inStock"] = inStock ?? null;
            TempData["SortBy"] = SortBy;
            int pageNumber = page ?? 1;
            int pageSize = 12;

            var query = await _unitOfWork.SanPham.GetAll(
                includeProperties: "DanhMuc,HinhAnhs",
                filter: x => x.TrangThai
            );

            if (!string.IsNullOrEmpty(findText))
            {
                query = query.Where(x => x.TenSanPham.ToLower().Contains(findText.ToLower()));
            }

            if (selectedCategories != null && selectedCategories.Any())
            {
                query = query.Where(x => selectedCategories.Any(select => select == x.DanhMucId));
            }

            if (minPrice.HasValue && maxPrice.HasValue && (minPrice.Value != 0 || maxPrice.Value != 0))
            {
                query = query.Where(x => x.Gia >= (minPrice ?? 0) && x.Gia <= (maxPrice ?? decimal.MaxValue));
            }

            if (inStock.HasValue)
            {
                query = inStock.Value ? query.Where(x => x.SoLuong > 0) : query.Where(x => x.SoLuong <= 0);
            }

            switch (SortBy)
            {
                case "best-selling":
                    query = query.OrderByDescending(x => x.SoLuong); // Giả sử có thuộc tính SoldCount
                    break;
                case "title-ascending":
                    query = query.OrderBy(x => x.TenSanPham);
                    break;
                case "title-descending":
                    query = query.OrderByDescending(x => x.TenSanPham);
                    break;
                case "price-ascending":
                    query = query.OrderBy(x => x.Gia);
                    break;
                case "price-descending":
                    query = query.OrderByDescending(x => x.Gia);
                    break;
                case "created-descending":
                    query = query.OrderByDescending(x => x.NgayTao); // Giả sử có thuộc tính CreatedDate
                    break;
                case "created-ascending":
                    query = query.OrderBy(x => x.NgayTao);
                    break;
                default: // "manual"
                    query = query.OrderBy(x => x.Id);
                    break;
            }

            var listProduct = query;

            
            var pagedListProduct = listProduct.OrderBy(x => x.Id).ToPagedList(pageNumber, pageSize);
            return PartialView(pagedListProduct);
        }


        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _unitOfWork.SanPham.Get(includeProperties: "DanhMuc,HinhAnhs,BinhLuans,DanhSachYeuThichs", filter: x => x.TrangThai && x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
