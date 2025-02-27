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
            ViewBag.DanTocList = new SelectList(db.DanTocs, "DanTocId", "TenDanToc");
            ViewBag.HuyenList = new SelectList(db.DanhMucHuyens, "HuyenId", "TenHuyen");
            ViewBag.XaList = new SelectList(db.DanhMucXas, "XaId", "TenXa");
            ViewBag.TinhList = new SelectList(db.DanhMucTinhs, "TinhId", "TenTinh");
            ViewBag.NgheNghiepList = new SelectList(db.NgheNghieps, "NgheNghiepId", "TenNgheNghiep");
            return View("Edit", employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                ModelState.AddModelError("", "ID nhân viên không hợp lệ.");
                await LoadDropdownLists();
                return View("Edit", employee);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm nhân viên hiện tại trong context để cập nhật
                    var existingEmployee = await db.Employees.FindAsync(id);
                    if (existingEmployee == null)
                    {
                        ModelState.AddModelError("", "Không tìm thấy nhân viên để cập nhật.");
                        await LoadDropdownLists();
                        return View("Edit", employee);
                    }

                    // Kiểm tra CCCD trùng lặp chỉ khi CCCD thay đổi
                    if (!string.IsNullOrEmpty(employee.Cccd) && employee.Cccd != existingEmployee.Cccd)
                    {
                        var existingCccd = await db.Employees
                            .AnyAsync(e => e.Cccd == employee.Cccd && e.EmployeeId != id);
                        if (existingCccd)
                        {
                            ModelState.AddModelError("Cccd", "CCCD đã tồn tại trong hệ thống.");
                            await LoadDropdownLists();
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
                    await LoadDropdownLists();
                    return View("Edit", employee);
                }
            }

            // Nếu ModelState không hợp lệ, load lại dropdown và trả lại view
            await LoadDropdownLists();
            return View("Edit", employee);
        }

        private async Task LoadDropdownLists()
        {
            ViewBag.DanTocList = new SelectList(await db.DanTocs.ToListAsync(), "DanTocId", "TenDanToc");
            ViewBag.HuyenList = new SelectList(await db.DanhMucHuyens.ToListAsync(), "HuyenId", "TenHuyen");
            ViewBag.XaList = new SelectList(await db.DanhMucXas.ToListAsync(), "XaId", "TenXa");
            ViewBag.TinhList = new SelectList(await db.DanhMucTinhs.ToListAsync(), "TinhId", "TenTinh");
            ViewBag.NgheNghiepList = new SelectList(await db.NgheNghieps.ToListAsync(), "NgheNghiepId", "TenNgheNghiep");
        }

        private bool EmployeeExists(int id)
        {
            return db.Employees.Any(e => e.EmployeeId == id);
        }
    }
}