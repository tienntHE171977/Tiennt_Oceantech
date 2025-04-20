using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Tiennthe171977_Oceanteach.Business;
using Tiennthe171977_Oceanteach.Models;

namespace Tiennthe171977_Oceanteach.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeBusiness _employeeBusiness;
        private readonly ILocationBusiness _locationBusiness; // Assuming you have this injected

        // Constructor with dependencies injected
        public EmployeeController(IEmployeeBusiness employeeBusiness, ILocationBusiness locationBusiness)
        {
            _employeeBusiness = employeeBusiness;
            _locationBusiness = locationBusiness;
        }

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            const int pageSize = 10;
            var employees = string.IsNullOrEmpty(searchTerm)
                ? await _employeeBusiness.GetEmployeesAsync(page, pageSize)
                : await _employeeBusiness.SearchEmployeesAsync(searchTerm);

            int totalRecords = string.IsNullOrEmpty(searchTerm)
                ? await _employeeBusiness.GetTotalEmployeesCountAsync()
                : employees.Count; // Khi tìm kiếm, tổng số bản ghi là số kết quả tìm kiếm

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;

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
            var employee = await _employeeBusiness.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Không tìm thấy nhân viên.");
            }

            var vanBangs = await _employeeBusiness.GetVanBangsByEmployeeIdAsync(employeeId);
            ViewBag.EmployeeId = employeeId;
            ViewBag.EmployeeName = employee.HoTen;

            // Tải danh sách tỉnh cho dropdown
            ViewBag.DanhSachTinh = await _employeeBusiness.GetDanhMucTinhsAsync();

            return View(vanBangs ?? new List<VanBang>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVanBang(VanBang vanBang)
        {
            if (!ModelState.IsValid)
            {
                return await HandleInvalidModelState(vanBang);
            }

            try
            {
                var result = await _employeeBusiness.AddVanBangAsync(vanBang.EmployeeId ?? 0, vanBang);
                if (!result)
                {
                    ModelState.AddModelError("", "Không thể thêm văn bằng.");
                    return await HandleInvalidModelState(vanBang);
                }

                return RedirectToAction("EditVanBang", new { employeeId = vanBang.EmployeeId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                return await HandleInvalidModelState(vanBang);
            }
        }

        private async Task<IActionResult> HandleInvalidModelState(VanBang vanBang)
        {
            ViewBag.DanhSachTinh = await _employeeBusiness.GetDanhMucTinhsAsync();
            var vanBangs = await _employeeBusiness.GetVanBangsByEmployeeIdAsync(vanBang.EmployeeId ?? 0);
            ViewBag.EmployeeId = vanBang.EmployeeId;

            return View("EditVanBang", vanBangs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVanBangs(int employeeId, List<VanBang> vanBangs)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DanhSachTinh = await _employeeBusiness.GetDanhMucTinhsAsync();
                ViewBag.EmployeeId = employeeId;
                return View("EditVanBang", vanBangs);
            }

            try
            {
                foreach (var vanBang in vanBangs)
                {
                    if (vanBang.VanBangId > 0)
                    {
                        // Cập nhật văn bằng đã tồn tại
                        await _employeeBusiness.UpdateVanBangAsync(employeeId, vanBang);
                    }
                    else
                    {
                        // Thêm mới văn bằng
                        vanBang.EmployeeId = employeeId;
                        await _employeeBusiness.AddVanBangAsync(employeeId, vanBang);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi cập nhật văn bằng: {ex.Message}");
                ViewBag.DanhSachTinh = await _employeeBusiness.GetDanhMucTinhsAsync();
                ViewBag.EmployeeId = employeeId;
                return View("EditVanBang", vanBangs);
            }

            return RedirectToAction("EditVanBang", new { employeeId });
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

        [HttpGet]
        public async Task<IActionResult> ExportEmployees(string type, string searchTerm)
        {
            List<Employee> employees;

            try
            {
                if (type == "search")
                {
                    // Export employees based on search criteria
                    employees = await _employeeBusiness.SearchEmployeesAsync(searchTerm ?? "");
                }
                else
                {
                    // Export all employees
                    employees = await _employeeBusiness.GetEmployeesAsync(1, int.MaxValue);
                }

                if (employees == null || !employees.Any())
                {
                    // Trả về một file Excel trống nếu không có dữ liệu
                    // hoặc có thể đưa ra thông báo
                    return Content("Không có dữ liệu để xuất");
                }

                // Generate Excel file
                var excelPackage = await GenerateExcelFile(employees);
                var stream = new MemoryStream();
                await excelPackage.SaveAsAsync(stream);
                stream.Position = 0;

                string fileName = $"Employees_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in ExportEmployees: {ex.Message}");
                // Trả về lỗi
                return Content($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportEmployees(string type, [FromForm] List<int> selectedIds, string searchTerm)
        {
            List<Employee> employees;

            if (type == "selected" && selectedIds != null && selectedIds.Count > 0)
            {
                // Export selected employees
                employees = await _employeeBusiness.GetEmployeesByIdsAsync(selectedIds);
            }
            else
            {
                // Export employees based on search criteria
                employees = await _employeeBusiness.SearchEmployeesAsync(searchTerm);
            }

            // Generate and return Excel file
            var excelPackage = await GenerateExcelFile(employees);
            var stream = new MemoryStream();
            await excelPackage.SaveAsAsync(stream);
            stream.Position = 0;

            string fileName = $"Employees_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private async Task<ExcelPackage> GenerateExcelFile(List<Employee> employees)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //ExcelPackage.License.SetNonCommercialPersonal("Tiennthe171977_Oceanteach");

            var excelPackage = new ExcelPackage();

            // Add a new worksheet to the workbook
            var worksheet = excelPackage.Workbook.Worksheets.Add("Employees");

            // Define headers
            var headers = new List<string>
            {
                "ID", "Họ Tên", "Ngày Sinh", "Tuổi", "Dân Tộc", "Nghề Nghiệp",
                "CCCD", "Số Điện Thoại", "Tỉnh/TP", "Quận/Huyện", "Phường/Xã", "Địa Chỉ Chi Tiết"
            };

            // Add headers
            for (int i = 0; i < headers.Count; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, headers.Count])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            }

            // Add data rows
            int row = 2;
            foreach (var emp in employees)
            {
                // Get location names
                string tinhName = "Không có";
                string huyenName = "Không có";
                string xaName = "Không có";

                if (emp.TinhId.HasValue)
                {
                    var tinh = await _locationBusiness.GetTinhByIdAsync(emp.TinhId.Value);
                    tinhName = tinh?.TenTinh ?? "Không có";
                }

                if (emp.HuyenId.HasValue)
                {
                    var huyen = await _locationBusiness.GetHuyenByIdAsync(emp.HuyenId.Value);
                    huyenName = huyen != null ? ((dynamic)huyen).TenHuyen : "Không có";
                }

                if (emp.XaId.HasValue)
                {
                    var xa = await _locationBusiness.GetXaByIdAsync(emp.XaId.Value);
                    xaName = xa != null ? ((dynamic)xa).TenXa : "Không có";
                }

                string danTocName = await GetDanTocNameAsync(emp.DanTocId);
                string ngheNghiepName = await GetNgheNghiepNameAsync(emp.NgheNghiepId);

                worksheet.Cells[row, 1].Value = emp.EmployeeId;
                worksheet.Cells[row, 2].Value = emp.HoTen;
                worksheet.Cells[row, 3].Value = emp.NgaySinh?.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 4].Value = emp.Tuoi;
                worksheet.Cells[row, 5].Value = danTocName;
                worksheet.Cells[row, 6].Value = ngheNghiepName;
                worksheet.Cells[row, 7].Value = emp.Cccd;
                worksheet.Cells[row, 8].Value = emp.SoDienThoai;
                worksheet.Cells[row, 9].Value = tinhName;
                worksheet.Cells[row, 10].Value = huyenName;
                worksheet.Cells[row, 11].Value = xaName;
                worksheet.Cells[row, 12].Value = emp.DiaChiCuThe;

                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return excelPackage;
        }

        private async Task<string> GetDanTocNameAsync(int? danTocId)
        {
            if (!danTocId.HasValue) return "Không có";

            var danToc = await _employeeBusiness.GetDanTocByIdAsync(danTocId.Value);
            return danToc?.TenDanToc ?? "Không có";
        }

        private async Task<string> GetNgheNghiepNameAsync(int? ngheNghiepId)
        {
            if (!ngheNghiepId.HasValue) return "Không có";

            var ngheNghiep = await _employeeBusiness.GetNgheNghiepByIdAsync(ngheNghiepId.Value);
            return ngheNghiep?.TenNgheNghiep ?? "Không có";
        }
    }
}