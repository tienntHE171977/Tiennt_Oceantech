using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using Tiennthe171977_Oceanteach.Business;
using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;

namespace Tiennthe171977_Oceanteach.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeBusiness _employeeBusiness;

        public HomeController(ILogger<HomeController> logger, IEmployeeBusiness employeeBusiness, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
            _employeeBusiness = employeeBusiness;
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownLists();
            return View(new Employee());
        }

        public IActionResult Location()
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
            var huyenList = await _employeeService.GetHuyenByTinhAsync(tinhId);
            return Json(huyenList.Select(h => new { h.HuyenId, h.TenHuyen }));
        }

        [HttpGet]
        public async Task<IActionResult> GetXaByHuyen(int huyenId)
        {
            var xaList = await _employeeService.GetXaByHuyenAsync(huyenId);
            return Json(xaList.Select(x => new { x.XaId, x.TenXa }));
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Employee employee)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        await LoadDropdownLists(employee.TinhId, employee.HuyenId);
        //        return View(employee);
        //    }

        //    // Validate mối quan hệ tỉnh-huyện-xã
        //    if (!await _employeeService.ValidateLocationAsync(employee.TinhId, employee.HuyenId, employee.XaId))
        //    {
        //        ModelState.AddModelError("", "Thông tin tỉnh, huyện, xã không hợp lệ.");
        //        await LoadDropdownLists(employee.TinhId, employee.HuyenId);
        //        return View(employee);
        //    }

        //    // Kiểm tra CCCD trùng lặp
        //    if (!string.IsNullOrEmpty(employee.Cccd))
        //    {
        //        if (await _employeeService.IsCccdExistsAsync(employee.Cccd))
        //        {
        //            ModelState.AddModelError("Cccd", "CCCD Đã Tồn Tại.");
        //            await LoadDropdownLists(employee.TinhId, employee.HuyenId);
        //            return View(employee);
        //        }
        //    }
        //    try
        //    {
        //        await _employeeService.CreateEmployeeAsync(employee);
        //        return RedirectToAction("Create");
        //    }
        //    catch (Exception ex) {
        //        ModelState.AddModelError("", "Có lỗi xảy ra khi thêm nhân viên: " + ex.Message);
        //        await LoadDropdownLists(employee.TinhId, employee.HuyenId);
        //        return View(employee);
        //    }
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View(employee);
            }

            try
            {
                await _employeeBusiness.CreateEmployeeAsync(employee);
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm nhân viên: " + ex.Message);
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View(employee);
            }
        }

        private async Task LoadDropdownLists(int? tinhId = null, int? huyenId = null)
        {
            ViewBag.TinhList = new SelectList(await _employeeService.GetDanhMucTinhsAsync(), "TinhId", "TenTinh", tinhId);
            ViewBag.HuyenList = new SelectList(
                await _employeeService.GetDanhMucHuyensAsync(tinhId), "HuyenId", "TenHuyen", huyenId);
            ViewBag.XaList = new SelectList(
                await _employeeService.GetDanhMucXasAsync(huyenId), "XaId", "TenXa");
            ViewBag.DanTocList = new SelectList(await _employeeService.GetDanTocsAsync(), "DanTocId", "TenDanToc");
            ViewBag.NgheNghiepList = new SelectList(await _employeeService.GetNgheNghiepsAsync(), "NgheNghiepId", "TenNgheNghiep");
        }

        //private async Task<bool> ValidateLocation(int? tinhId, int? huyenId, int? xaId)
        //{
        //    if (!tinhId.HasValue || !huyenId.HasValue || !xaId.HasValue) return false;

        //    var huyen = await db.DanhMucHuyens
        //        .FirstOrDefaultAsync(h => h.HuyenId == huyenId && h.TinhId == tinhId);
        //    if (huyen == null) return false;

        //    var xa = await db.DanhMucXas
        //        .FirstOrDefaultAsync(x => x.XaId == xaId && x.HuyenId == huyenId);
        //    return xa != null;
        //}
    }
}