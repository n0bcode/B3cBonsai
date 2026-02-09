using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CayCuaToiController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public CayCuaToiController(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myTrees = await _unitOfWork.CayCuaToi.GetAll(u => u.NguoiDungId == userId, includeProperties: "SanPham");
            return View(myTrees);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tree = await _unitOfWork.CayCuaToi.Get(u => u.Id == id && u.NguoiDungId == userId, includeProperties: "SanPham,NhatKyCays");

            if (tree == null)
            {
                return NotFound();
            }

            // Order journals by Newest first
            tree.NhatKyCays = tree.NhatKyCays.OrderByDescending(j => j.NgayTao).ToList();

            return View(tree);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CayCuaToi tree = new CayCuaToi();

            // Load list of products for linking (optional)
            // For now, just load all active products. In future, restrict to purchased.
            ViewBag.SanPhams = (await _unitOfWork.SanPham.GetAll(s => s.TrangThai == true))
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.TenSanPham });

            if (id == null || id == 0)
            {
                // Create
                tree.NguoiDungId = userId;
                return View(tree);
            }
            else
            {
                // Edit
                tree = await _unitOfWork.CayCuaToi.Get(u => u.Id == id && u.NguoiDungId == userId);
                if (tree == null)
                {
                    return NotFound();
                }
                return View(tree);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CayCuaToi tree, IFormFile? file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            tree.NguoiDungId = userId; // Ensure User ownership

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    if (tree.HinhAnhDaiDien != null)
                    {
                        await _fileStorageService.DeleteFileAsync(tree.HinhAnhDaiDien);
                    }
                    tree.HinhAnhDaiDien = await _fileStorageService.StoreFileAsync(file, "caycuatoi");
                }

                if (tree.Id == 0)
                {
                    _unitOfWork.CayCuaToi.Add(tree);
                }
                else
                {
                    // For update, we need to be careful not to overwrite things if simpler update mapping
                    // But here model binding should work for submitted fields.
                    // Ideally fetch existing and update fields, but repository Update handles it if attached?
                    // Safe way:
                    var objFromDb = await _unitOfWork.CayCuaToi.Get(u => u.Id == tree.Id && u.NguoiDungId == userId);
                    if (objFromDb != null)
                    {
                        objFromDb.TenCay = tree.TenCay;
                        objFromDb.GhiChu = tree.GhiChu;
                        objFromDb.TrangThai = tree.TrangThai;
                        objFromDb.SanPhamId = tree.SanPhamId;
                        objFromDb.NgayMua = tree.NgayMua;
                        if(file != null) objFromDb.HinhAnhDaiDien = tree.HinhAnhDaiDien;
                        
                        _unitOfWork.CayCuaToi.Update(objFromDb);
                    }
                    else
                    {
                         // Security check failed or not found
                         return NotFound();
                    }
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.SanPhams = (await _unitOfWork.SanPham.GetAll(s => s.TrangThai == true))
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.TenSanPham });
            return View(tree);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddJournalEntry(int CayCuaToiId, string NoiDung, string? GiaiDoanPhatTrien, IFormFile? journalFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tree = await _unitOfWork.CayCuaToi.Get(u => u.Id == CayCuaToiId && u.NguoiDungId == userId);
            
            if (tree == null) return NotFound();

            var journal = new NhatKyCay
            {
                CayCuaToiId = CayCuaToiId,
                NoiDung = NoiDung,
                GiaiDoanPhatTrien = GiaiDoanPhatTrien,
                NgayTao = DateTimeOffset.UtcNow
            };

            if (journalFile != null)
            {
                journal.HinhAnh = await _fileStorageService.StoreFileAsync(journalFile, "nhatkycay");
            }

            _unitOfWork.NhatKyCay.Add(journal);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Details), new { id = CayCuaToiId });
        }

        #region API CALLS
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tree = await _unitOfWork.CayCuaToi.Get(u => u.Id == id && u.NguoiDungId == userId);
            
            if (tree == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Optional: Delete images associated
            if (tree.HinhAnhDaiDien != null)
            {
                await _fileStorageService.DeleteFileAsync(tree.HinhAnhDaiDien);
            }
            // Delete journal images? Loop through journals
            var journals = await _unitOfWork.NhatKyCay.GetAll(j => j.CayCuaToiId == id);
            foreach(var j in journals)
            {
                 if(j.HinhAnh != null) await _fileStorageService.DeleteFileAsync(j.HinhAnh);
            }

            _unitOfWork.CayCuaToi.Remove(tree);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
