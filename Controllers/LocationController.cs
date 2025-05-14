using Microsoft.AspNetCore.Mvc;
using Tiennthe171977_Oceanteach.Business;
using Tiennthe171977_Oceanteach.Models;
using FluentValidation;

namespace Tiennthe171977_Oceanteach.Controllers
{
    public class LocationController : Controller
    {
        private readonly ILocationBusiness _locationBusiness;

        public LocationController(ILocationBusiness locationBusiness)
        {
            _locationBusiness = locationBusiness;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTinh(string searchTerm = null)
        {
            try
            {
                var danhSachTinh = await _locationBusiness.GetAllTinhAsync(searchTerm);
                return Json(new { success = true, data = danhSachTinh });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTinhById(int id)
        {
            try
            {
                var tinh = await _locationBusiness.GetTinhByIdAsync(id);
                if (tinh == null)
                    return Json(new { success = false, message = "Không tìm thấy tỉnh" });

                return Json(new { success = true, data = tinh });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTinh([FromBody] DanhMucTinh tinh)
        {
            try
            {
                var createdTinh = await _locationBusiness.CreateTinhAsync(tinh);
                return Json(new { success = true, data = new { tinhId = createdTinh.TinhId, tenTinh = createdTinh.TenTinh } });

            }
            catch (ValidationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTinh([FromBody] DanhMucTinh tinh)
        {
            try
            {
                var result = await _locationBusiness.UpdateTinhAsync(tinh);
                if (!result)
                    return Json(new { success = false, message = "Cập nhật tỉnh thất bại" });

                return Json(new { success = true });
            }
            catch (ValidationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTinh(int id)
        {
            try
            {
                var result = await _locationBusiness.DeleteTinhWithDependenciesAsync(id);
                if (!result)
                    return Json(new { success = false, message = "Xóa tỉnh thất bại" });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHuyen(int? tinhId = null, string searchTerm = null)
        {
            try
            {
                var danhSachHuyen = await _locationBusiness.GetAllHuyenAsync(tinhId, searchTerm);
                return Json(new { success = true, data = danhSachHuyen });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetHuyenById(int id)
        {
            try
            {
                var huyen = await _locationBusiness.GetHuyenByIdAsync(id);
                if (huyen == null)
                    return Json(new { success = false, message = "Không tìm thấy huyện" });

                return Json(new { success = true, data = huyen });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetHuyensByTinhId(int tinhId)
        {
            try
            {
                var danhSachHuyen = await _locationBusiness.GetHuyensByTinhIdAsync(tinhId);
                return Json(new { success = true, data = danhSachHuyen });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateHuyen([FromBody] DanhMucHuyen huyen)
        {
            try
            {
                var createdHuyen = await _locationBusiness.CreateHuyenAsync(huyen);
                return Json(new { success = true, data = new { huyenId = createdHuyen.HuyenId, tenHuyen = createdHuyen.TenHuyen } });

            }
            catch (ValidationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateHuyen([FromBody] DanhMucHuyen huyen)
        {
            try
            {
                var result = await _locationBusiness.UpdateHuyenAsync(huyen);
                if (!result)
                    return Json(new { success = false, message = "Cập nhật huyện thất bại" });

                return Json(new { success = true });
            }
            catch (ValidationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteHuyen(int id)
        {
            try
            {
                var result = await _locationBusiness.DeleteHuyenWithDependenciesAsync(id);
                if (!result)
                    return Json(new { success = false, message = "Xóa huyện thất bại" });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllXa(int? tinhId = null, int? huyenId = null, string searchTerm = null)
        {
            try
            {
                var danhSachXa = await _locationBusiness.GetAllXaAsync(tinhId, huyenId, searchTerm);
                return Json(new { success = true, data = danhSachXa });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetXaById(int id)
        {
            try
            {
                var xa = await _locationBusiness.GetXaByIdAsync(id);
                if (xa == null)
                    return Json(new { success = false, message = "Không tìm thấy xã" });

                return Json(new { success = true, data = xa });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetXasByHuyenId(int huyenId)
        {
            try
            {
                var danhSachXa = await _locationBusiness.GetXasByHuyenIdAsync(huyenId);
                return Json(new { success = true, data = danhSachXa });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateXa([FromBody] DanhMucXa xa)
        {
            try
            {
                var createdXa = await _locationBusiness.CreateXaAsync(xa);
                return Json(new { success = true, data = new { xaId = createdXa.XaId, tenXa = createdXa.TenXa } });

            }
            catch (ValidationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateXa([FromBody] DanhMucXa xa)
        {
            try
            {
                var result = await _locationBusiness.UpdateXaAsync(xa);
                if (!result)
                    return Json(new { success = false, message = "Cập nhật xã thất bại" });

                return Json(new { success = true });
            }
            catch (ValidationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteXa(int id)
        {
            try
            {
                var result = await _locationBusiness.DeleteXaWithDependenciesAsync(id);
                if (!result)
                    return Json(new { success = false, message = "Xóa xã thất bại" });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}