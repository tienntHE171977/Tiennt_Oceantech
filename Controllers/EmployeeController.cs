using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tiennthe171977_Oceanteach.Business;
using Tiennthe171977_Oceanteach.Models;

namespace Tiennthe171977_Oceanteach.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeBusiness _employeeBusiness;

        public EmployeeController(IEmployeeBusiness employeeBusiness)
        {
            _employeeBusiness = employeeBusiness;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var employees = await _employeeBusiness.GetEmployeesAsync(page, pageSize);
            int totalRecords = await _employeeBusiness.GetTotalEmployeesCountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(employees);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var employee = await _employeeBusiness.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound("Không tìm thấy nhân viên.");
            }
            return View(employee);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeBusiness.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound("Không tìm thấy nhân viên.");
            }

            await LoadDropdownLists(employee.TinhId, employee.HuyenId);
            return View(employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTinh(int tinhId)
        {
            var result = await _employeeBusiness.DeleteTinhAsync(tinhId);
            if (!result)
            {
                return NotFound("Không tìm thấy Tỉnh để xóa.");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                ModelState.AddModelError("", "ID nhân viên không hợp lệ.");
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View(employee);
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View(employee);
            }

            try
            {
                var result = await _employeeBusiness.UpdateEmployeeAsync(employee);
                if (!result)
                {
                    ModelState.AddModelError("", "Không thể cập nhật nhân viên.");
                    await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                    return View(employee);
                }

                return RedirectToAction("Edit");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View(employee);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _employeeBusiness.DeleteEmployeeAsync(id);
            if (!result)
            {
                return NotFound("Không tìm thấy nhân viên để xóa.");
            }

            return RedirectToAction("Index");
        }
        [Route("Employee/EditVanBang/{employeeId:int}")]
        [HttpGet]
        public async Task<IActionResult> EditVanBang(int employeeId)
        {
            var vanBangs = await _employeeBusiness.GetVanBangsByEmployeeIdAsync(employeeId);
            if (vanBangs == null || !vanBangs.Any())
            {
                return NotFound("Không tìm thấy văn bằng nào cho nhân viên này.");
            }

            ViewBag.EmployeeId = employeeId;
            return View(vanBangs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVanBang(VanBang vanBang)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TinhList = new SelectList(await _employeeBusiness.GetDanhMucTinhsAsync(), "TinhId", "TenTinh", vanBang.DonViCap);
                return View(vanBang);
            }

            var result = await _employeeBusiness.UpdateVanBangAsync(vanBang.EmployeeId ?? 0, vanBang);
            if (!result)
            {
                return NotFound("Không tìm thấy văn bằng để cập nhật.");
            }

            return RedirectToAction("EditVanBang", new { employeeId = vanBang.EmployeeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVanBang(int employeeId, int vanBangId)
        {
            var result = await _employeeBusiness.DeleteVanBangAsync(employeeId, vanBangId);
            if (!result)
            {
                return NotFound("Không tìm thấy văn bằng để xóa.");
            }

            return RedirectToAction("EditVanBang", new { employeeId });
        }

        [HttpGet]
        public async Task<IActionResult> GetHuyenByTinh(int tinhId)
        {
            var huyenList = await _employeeBusiness.GetHuyenByTinhAsync(tinhId);
            return Json(huyenList);
        }

        [HttpGet]
        public async Task<IActionResult> GetXaByHuyen(int huyenId)
        {
            var xaList = await _employeeBusiness.GetXaByHuyenAsync(huyenId);
            return Json(xaList);
        }

        private async Task LoadDropdownLists(int? tinhId = null, int? huyenId = null)
        {
            ViewBag.TinhList = new SelectList(await _employeeBusiness.GetDanhMucTinhsAsync(), "TinhId", "TenTinh", tinhId);
            ViewBag.HuyenList = new SelectList(await _employeeBusiness.GetDanhMucHuyensAsync(tinhId), "HuyenId", "TenHuyen", huyenId);
            ViewBag.XaList = new SelectList(await _employeeBusiness.GetDanhMucXasAsync(huyenId), "XaId", "TenXa");
            ViewBag.DanTocList = new SelectList(await _employeeBusiness.GetDanTocsAsync(), "DanTocId", "TenDanToc");
            ViewBag.NgheNghiepList = new SelectList(await _employeeBusiness.GetNgheNghiepsAsync(), "NgheNghiepId", "TenNgheNghiep");
        }
    }
}
