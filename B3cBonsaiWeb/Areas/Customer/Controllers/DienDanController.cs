using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System;
using X.PagedList;
using X.PagedList.Extensions;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DienDanController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHtmlSanitizerService _sanitizer;

        public DienDanController(IUnitOfWork unitOfWork, IHtmlSanitizerService sanitizer)
        {
            _unitOfWork = unitOfWork;
            _sanitizer = sanitizer;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var threads = await _unitOfWork.ChuDe.GetAll(includeProperties: "DanhMucDienDan,NguoiDungUngDung,BaiViets");
            ViewBag.HotThreads = threads.OrderByDescending(t => t.LuotXem).Take(5).ToList();
            return View(threads.OrderByDescending(t => t.NgayTao).ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> ChiTiet(int id, string slug)
        {
            // Remove BaiViets from include to prevent loading all replies efficiently (Pagination prep)
            var thread = await _unitOfWork.ChuDe.Get(t => t.Id == id, includeProperties: "DanhMucDienDan,NguoiDungUngDung");

            if (thread == null) return NotFound();

            // Simple slug check for SEO redirect
            if (!string.IsNullOrEmpty(slug) && thread.Slug != slug)
            {
                return RedirectToAction(nameof(ChiTiet), new { id = thread.Id, slug = thread.Slug });
            }

            // Load replies separately - ready for pagination (Skip/Take)
            // Currently loading all, but detached from main query
            thread.BaiViets = (await _unitOfWork.BaiViet.GetAll(b => b.ChuDeId == id, includeProperties: "NguoiDungUngDung")).ToList();

            // Increment View Count
            _unitOfWork.ChuDe.IncrementViewCount(thread.Id);

            return View(thread);
        }

        [Authorize]
        public async Task<IActionResult> TaoChuDe()
        {
            ViewBag.Categories = (await _unitOfWork.DanhMucDienDan.GetAll())
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.TenDanhMuc });
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoChuDe(ChuDe chuDe)
        {
            if (ModelState.IsValid)
            {
                chuDe.NguoiDungId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                chuDe.Slug = SD.GenerateSlug(chuDe.TieuDe);
                chuDe.NgayTao = DateTimeOffset.UtcNow;
                chuDe.LuotXem = 0;
                chuDe.TrangThai = true;

                // Sanitize content and title to prevent stored XSS
                chuDe.TieuDe = _sanitizer.Sanitize(chuDe.TieuDe);
                chuDe.NoiDung = _sanitizer.Sanitize(chuDe.NoiDung);

                _unitOfWork.ChuDe.Add(chuDe);
                _unitOfWork.Save();
                return RedirectToAction(nameof(ChiTiet), new { id = chuDe.Id, slug = chuDe.Slug });
            }
            ViewBag.Categories = (await _unitOfWork.DanhMucDienDan.GetAll())
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.TenDanhMuc });
            return View(chuDe);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraLoi(int ChuDeId, string NoiDung)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chuDe = await _unitOfWork.ChuDe.Get(c => c.Id == ChuDeId);

            if (chuDe == null) return NotFound();

            var sanitizedContent = _sanitizer.Sanitize(NoiDung);

            if (string.IsNullOrWhiteSpace(sanitizedContent))
            {
                return RedirectToAction(nameof(ChiTiet), new { id = ChuDeId, slug = chuDe.Slug });
            }

            var baiViet = new BaiViet
            {
                ChuDeId = ChuDeId,
                NguoiDungId = userId,
                NoiDung = sanitizedContent,
                NgayTao = DateTimeOffset.UtcNow
            };

            _unitOfWork.BaiViet.Add(baiViet);
            _unitOfWork.Save();

            return RedirectToAction(nameof(ChiTiet), new { id = ChuDeId, slug = chuDe.Slug });
        }




    }
}
