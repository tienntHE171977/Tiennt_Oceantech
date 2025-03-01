using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tiennthe171977_Oceanteach.Models;

namespace Tiennthe171977_Oceanteach.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly OceantechContext db;

        public EmployeeController()
        {
            db = new OceantechContext();
        }

        public ActionResult Index(int page = 1)
        {
            const int pageSize = 10;
            var employees = db.Employees
                .OrderBy(e => e.EmployeeId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            int totalRecords = db.Employees.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(employees.ToList());
        }

        public IActionResult Delete(int id)
        {
            var employee = db.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }
            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
            var employee = await db.Employees
                .Include(e => e.DanToc)
                .Include(e => e.NgheNghiep)
                .Include(e => e.Tinh)
                .Include(e => e.Huyen)
                .Include(e => e.Xa)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
            return View(employee);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var employee = await db.Employees
                .Include(e => e.DanToc)
                .Include(e => e.NgheNghiep)
                .Include(e => e.Tinh)
                .Include(e => e.Huyen)
                .Include(e => e.Xa)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (employee == null) { return NotFound(); }
            await LoadDropdownLists(employee.TinhId, employee.HuyenId);
            return View("Edit", employee);
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
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                ModelState.AddModelError("", "ID nhân viên không hợp lệ.");
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View("Edit", employee);
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View("Edit", employee);
            }

            // Validate mối quan hệ tỉnh-huyện-xã
            if (!await ValidateLocation(employee.TinhId, employee.HuyenId, employee.XaId))
            {
                ModelState.AddModelError("", "Thông tin tỉnh, huyện, xã không hợp lệ.");
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View("Edit", employee);
            }

            try
            {
                var existingEmployee = await db.Employees.FindAsync(id);
                if (existingEmployee == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy nhân viên để cập nhật.");
                    await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                    return View("Edit", employee);
                }

                // Kiểm tra CCCD trùng lặp
                if (!string.IsNullOrEmpty(employee.Cccd) && employee.Cccd != existingEmployee.Cccd)
                {
                    var existingCccd = await db.Employees
                        .AnyAsync(e => e.Cccd == employee.Cccd && e.EmployeeId != id);
                    if (existingCccd)
                    {
                        ModelState.AddModelError("Cccd", "CCCD đã tồn tại trong hệ thống.");
                        await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                        return View("Edit", employee);
                    }
                }

                // Cập nhật các trường
                existingEmployee.HoTen = employee.HoTen;
                existingEmployee.NgaySinh = employee.NgaySinh;
                existingEmployee.Tuoi = employee.Tuoi;
                existingEmployee.DanTocId = employee.DanTocId;
                existingEmployee.NgheNghiepId = employee.NgheNghiepId;
                existingEmployee.Cccd = string.IsNullOrEmpty(employee.Cccd) ? null : employee.Cccd;
                existingEmployee.SoDienThoai = string.IsNullOrEmpty(employee.SoDienThoai) ? null : employee.SoDienThoai;
                existingEmployee.TinhId = employee.TinhId;
                existingEmployee.HuyenId = employee.HuyenId;
                existingEmployee.XaId = employee.XaId;
                existingEmployee.DiaChiCuThe = employee.DiaChiCuThe;

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.EmployeeId))
                {
                    ModelState.AddModelError("", "Nhân viên không tồn tại hoặc đã bị xóa.");
                }
                else
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật. Vui lòng thử lại.");
                }
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View("Edit", employee);
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

        private bool EmployeeExists(int id)
        {
            return db.Employees.Any(e => e.EmployeeId == id);
        }

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