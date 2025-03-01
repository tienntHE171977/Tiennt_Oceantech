using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tiennthe171977_Oceanteach.Models;

namespace Tiennthe171977_Oceanteach.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly OceantechContext db;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            db = new OceantechContext();
        }



        public async Task<IActionResult> Create()
        {
            await LoadDropdownLists();
            return View(new Employee());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        

        [HttpGet]
        public async Task<IActionResult> GetHuyenByTinh(int tinhId)
        {
            var huyenList = await db.DanhMucHuyens
                .Where(h => h.TinhId == tinhId)
                .Select(h => new { h.HuyenId, h.TenHuyen })
                .ToListAsync();
            return Json(huyenList);
        }

        [HttpGet]
        public async Task<IActionResult> GetXaByHuyen(int huyenId)
        {
            var xaList = await db.DanhMucXas
                .Where(x => x.HuyenId == huyenId)
                .Select(x => new { x.XaId, x.TenXa })
                .ToListAsync();
            return Json(xaList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View(employee);
            }

            // Validate mối quan hệ tỉnh-huyện-xã
            if (!await ValidateLocation(employee.TinhId, employee.HuyenId, employee.XaId))
            {
                ModelState.AddModelError("", "Thông tin tỉnh, huyện, xã không hợp lệ.");
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View(employee);
            }

            // Kiểm tra CCCD trùng lặp
            if (!string.IsNullOrEmpty(employee.Cccd))
            {
                if (await db.Employees.AnyAsync(e => e.Cccd == employee.Cccd))
                {
                    ModelState.AddModelError("Cccd", "CCCD Đã Tồn Tại.");
                    await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                    return View(employee);
                }
            }
            try
            {
                db.Employees.Add(employee);
                await db.SaveChangesAsync();
                return RedirectToAction("Create");
            }
            catch (Exception ex) {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm nhân viên: " + ex.Message);
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View(employee);
            }
        }
        

        private async Task LoadDropdownLists(int? tinhId = null, int? huyenId = null)
        {
            ViewBag.TinhList = new SelectList(await db.DanhMucTinhs.ToListAsync(), "TinhId", "TenTinh", tinhId);
            ViewBag.HuyenList = new SelectList(
                await db.DanhMucHuyens.Where(h => !tinhId.HasValue || h.TinhId == tinhId).ToListAsync(),
                "HuyenId", "TenHuyen", huyenId);
            ViewBag.XaList = new SelectList(
                await db.DanhMucXas.Where(x => !huyenId.HasValue || x.HuyenId == huyenId).ToListAsync(),
                "XaId", "TenXa");
            ViewBag.DanTocList = new SelectList(await db.DanTocs.ToListAsync(), "DanTocId", "TenDanToc");
            ViewBag.NgheNghiepList = new SelectList(await db.NgheNghieps.ToListAsync(), "NgheNghiepId", "TenNgheNghiep");
        }

        //private bool EmployeeExists(int id)
        //{
        //    return db.Employees.Any(e => e.EmployeeId == id);
        //}

        private async Task<bool> ValidateLocation(int? tinhId, int? huyenId, int? xaId)
        {
            if (!tinhId.HasValue || !huyenId.HasValue || !xaId.HasValue) return false;

            var huyen = await db.DanhMucHuyens
                .FirstOrDefaultAsync(h => h.HuyenId == huyenId && h.TinhId == tinhId);
            if (huyen == null) return false;

            var xa = await db.DanhMucXas
                .FirstOrDefaultAsync(x => x.XaId == xaId && x.HuyenId == huyenId);
            return xa != null;
        }
    }
}