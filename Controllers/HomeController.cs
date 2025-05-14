using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using Tiennthe171977_Oceanteach.Business;
using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;
using FluentValidation;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            try
            {
                var result = await _employeeBusiness.CreateEmployeeAsync(employee);
                if (result)
                {
                    TempData["SuccessMessage"] = "Thêm nhân viên thành công";
                    return RedirectToAction("Create");
                }

                ModelState.AddModelError("", "Không thể thêm nhân viên");
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View(employee);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadDropdownLists(employee.TinhId, employee.HuyenId);
                return View(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo mới NV");
                ModelState.AddModelError("", ex.Message);
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
    }
}