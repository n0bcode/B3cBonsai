﻿using Microsoft.AspNetCore.Mvc;
using B3cBonsai.Models;
using Microsoft.EntityFrameworkCore;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Globalization;
using System.Text;
using NuGet.ProjectModel;
using B3cBonsai.Models.ViewModels;
using B3cBonsai.Utility;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ClientProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ClientProductController> _logger;

        public ClientProductController(IUnitOfWork context, ILogger<ClientProductController> logger, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = context;
            _logger = logger;
            _userManager = userManager;
        }

        // Trang chủ: Hiển thị các sản phẩm mới nhất
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả sản phẩm, bao gồm thông tin danh mục và hình ảnh liên quan
            var products = await _unitOfWork.SanPham.GetAll(
                includeProperties: "DanhMuc,HinhAnhs",
                filter: x => x.TrangThai == true // Chỉ lấy sản phẩm có trạng thái là active
            );

            // Lấy 6 sản phẩm mới nhất
            var latestProducts = products.OrderByDescending(x => x.NgayTao).Take(6).ToList();

            // Truyền các sản phẩm mới nhất vào ViewBag để sử dụng trong view
            ViewBag.LatestProducts = latestProducts;

            return View();
        }

        // Danh sách sản phẩm có phân trang và bộ lọc
        [HttpGet]
        public async Task<IActionResult> ListPagedProduct(
            int? page,
            string? findText,
            int[]? selectedCategories,
            int? minPrice,
            int? maxPrice,
            bool? inStock,
            string? sortBy)
        {
            // Lưu trữ tạm thời các tham số bộ lọc
            TempData["findText"] = findText ?? "";
            TempData["selectedCategories"] = selectedCategories;
            TempData["minPrice"] = minPrice ?? 0;
            TempData["maxPrice"] = maxPrice ?? int.MaxValue;
            TempData["inStock"] = inStock;
            TempData["SortBy"] = sortBy;

            int pageNumber = page ?? 1;
            int pageSize = 12;

            // Truy vấn sản phẩm
            var productQuery = await _unitOfWork.SanPham.GetAll(
                includeProperties: "DanhMuc,HinhAnhs",
                filter: x => x.TrangThai
            );

            // Truy vấn sản phẩm combo
            var comboQuery = await _unitOfWork.ComboSanPham.GetAll(
                includeProperties: "ChiTietCombos",
                filter: x => x.TrangThai
            );

            // Áp dụng bộ lọc
            if (!string.IsNullOrEmpty(findText))
            {
                var normalizedFindText = RemoveDiacritics(findText.ToLower());
                productQuery = productQuery.Where(x => RemoveDiacritics(x.TenSanPham.ToLower()).Contains(normalizedFindText));
                comboQuery = comboQuery.Where(x => RemoveDiacritics(x.TenCombo.ToLower()).Contains(normalizedFindText));
            }

            if (selectedCategories != null && selectedCategories.Any())
            {
                productQuery = productQuery.Where(x => selectedCategories.Contains(x.DanhMucId));
                comboQuery = comboQuery.Where(x => x.ChiTietCombos.Any(detail =>
                    selectedCategories.Contains(detail.SanPham.DanhMucId)));
            }

            if (minPrice.HasValue || maxPrice.HasValue)
            {
                int minimumPrice = minPrice ?? 0;
                int maximumPrice = maxPrice ?? int.MaxValue;
                productQuery = productQuery.Where(x => x.Gia >= minimumPrice && x.Gia <= maximumPrice);
                comboQuery = comboQuery.Where(x => x.TongGia >= minimumPrice && x.TongGia <= maximumPrice);
            }

            if (inStock.HasValue)
            {
                productQuery = inStock.Value
                    ? productQuery.Where(x => x.SoLuong > 0)
                    : productQuery.Where(x => x.SoLuong <= 0);

                comboQuery = inStock.Value
                    ? comboQuery.Where(x => x.SoLuong > 0)
                    : comboQuery.Where(x => x.SoLuong <= 0);
            }




            // Kết hợp và phân trang kết quả
            var sanPhamOrComboVMs = productQuery.Select(p => new SanPhamOrComboVM
            { SanPham = p, LoaiDoiTuong = SD.ObjectDetailOrder_SanPham }).ToList();

            sanPhamOrComboVMs.AddRange(comboQuery.Select(c => new SanPhamOrComboVM
            { ComboSanPham = c, LoaiDoiTuong = SD.ObjectDetailOrder_Combo }));

            var pagedList = sanPhamOrComboVMs.ToPagedList(pageNumber, pageSize);
            return PartialView(pagedList);
        }

        // Áp dụng logic sắp xếp
        private IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortBy) where T : class
        {
            return sortBy switch
            {
                "best-selling" => query.OrderByDescending(x => EF.Property<int>(x, "SoLuong")),
                "title-ascending" => query.OrderBy(x => EF.Property<string>(x, "TenCombo")),
                "title-descending" => query.OrderByDescending(x => EF.Property<string>(x, "TenCombo")),
                "price-ascending" => query.OrderBy(x => EF.Property<int>(x, "Gia")),
                "price-descending" => query.OrderByDescending(x => EF.Property<int>(x, "Gia")),
                "created-ascending" => query.OrderBy(x => EF.Property<DateTime>(x, "NgayTao")),
                "created-descending" => query.OrderByDescending(x => EF.Property<DateTime>(x, "NgayTao")),
                _ => query.OrderBy(x => EF.Property<int>(x, "Id"))
            };
        }

        // Bỏ dấu cho văn bản tiếng Việt
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }


        // Chi tiết sản phẩm
        [HttpGet]
        public async Task<IActionResult> Detail(int id, string typeObject = SD.ObjectDetailOrder_SanPham)
        {
            if (typeObject == SD.ObjectDetailOrder_SanPham)
            {
                // Lấy sản phẩm và các thông tin liên quan như danh mục, hình ảnh, bình luận, yêu thích
                var product = await _unitOfWork.SanPham.Get(
                    includeProperties: "DanhMuc,HinhAnhs,BinhLuans,DanhSachYeuThichs",
                    filter: x => x.TrangThai && x.Id == id
                );

                if (product == null)
                {
                    return NotFound(); // Sản phẩm không tìm thấy
                }

                return View("DetailProduct", product);
            }
            else
            {
                // Lấy sản phẩm và các thông tin liên quan như danh mục, hình ảnh, bình luận, yêu thích
                var combo = await _unitOfWork.ComboSanPham.Get(
                    includeProperties: "ChiTietCombos.SanPham",
                    filter: x => x.TrangThai && x.Id == id
                );

                if (combo == null)
                {
                    return NotFound(); // Sản phẩm không tìm thấy
                }

                return View("DetailCombo", combo);

            }
        }


        // Xem nhanh thông tin sản phẩm
        public async Task<IActionResult> QuickView(int? id)
        {
            return PartialView(await _unitOfWork.SanPham.Get(filter: sp => sp.Id == id, includeProperties: "DanhMuc,HinhAnhs"));
        }

        // Xem nhanh thông tin ComBo sản phẩm
        // Controller
        public async Task<IActionResult> QuickViewComBo(int? id)
        {
            if (id == null) return NotFound();

            var combo = await _unitOfWork.ComboSanPham.Get(
                filter: sp => sp.Id == id,
                includeProperties: "ChiTietCombos.SanPham"
            );

            if (combo == null) return NotFound();

            return PartialView("QuickViewComBo", combo);
        }

    }
}
