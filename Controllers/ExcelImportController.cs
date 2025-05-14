using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using Tiennthe171977_Oceanteach.Business;
using Tiennthe171977_Oceanteach.Models;
using FluentValidation;
using FluentValidation.Results;

namespace Tiennthe171977_Oceanteach.Controllers
{
    public class ExcelImportController : Controller
    {
        private readonly IEmployeeBusiness _employeeBusiness;
        private readonly IValidator<Employee> _employeeValidator;

        public ExcelImportController(IEmployeeBusiness employeeBusiness, IValidator<Employee> employeeValidator)
        {
            _employeeBusiness = employeeBusiness;
            _employeeValidator = employeeValidator;
        }

        public async Task<IActionResult> Import()
        {
            ViewBag.DanTocList = new SelectList(await _employeeBusiness.GetDanTocsAsync(), "DanTocId", "TenDanToc");
            ViewBag.NgheNghiepList = new SelectList(await _employeeBusiness.GetNgheNghiepsAsync(), "NgheNghiepId", "TenNgheNghiep");
            ViewBag.TinhList = new SelectList(await _employeeBusiness.GetDanhMucTinhsAsync(), "TinhId", "TenTinh");

            return View();
        }

        public async Task<IActionResult> DownloadTemplate()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Employees");

                    var headers = new List<string>
                    {
                        "Họ tên", "Ngày sinh", "Tuổi", "Dân tộc", "Nghề nghiệp",
                        "CCCD", "Số điện thoại", "Tỉnh/Thành phố", "Quận/Huyện",
                        "Xã/Phường", "Địa chỉ cụ thể"
                    };

                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    for (int i = 1; i <= headers.Count; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    var danTocs = await _employeeBusiness.GetDanTocsAsync();
                    var ngheNghieps = await _employeeBusiness.GetNgheNghiepsAsync();
                    var tinhs = await _employeeBusiness.GetDanhMucTinhsAsync();

                    worksheet.Cells[2, 1].Value = "Nguyễn Văn A";
                    worksheet.Cells[2, 2].Value = DateTime.Now.AddYears(-30).ToString("dd/MM/yyyy");
                    worksheet.Cells[2, 3].Value = 30;
                    worksheet.Cells[2, 4].Value = danTocs.FirstOrDefault()?.TenDanToc ?? "Kinh";
                    worksheet.Cells[2, 5].Value = ngheNghieps.FirstOrDefault()?.TenNgheNghiep ?? "Giáo viên";
                    worksheet.Cells[2, 6].Value = "012345678901";
                    worksheet.Cells[2, 7].Value = "0912345678";

                    var exampleTinh = tinhs.FirstOrDefault();
                    if (exampleTinh != null)
                    {
                        worksheet.Cells[2, 8].Value = exampleTinh.TenTinh;

                        var huyens = await _employeeBusiness.GetHuyenByTinhAsync(exampleTinh.TinhId);
                        var exampleHuyen = huyens.FirstOrDefault();
                        if (exampleHuyen != null)
                        {
                            worksheet.Cells[2, 9].Value = exampleHuyen.TenHuyen;

                            var xas = await _employeeBusiness.GetXaByHuyenAsync(exampleHuyen.HuyenId);
                            var exampleXa = xas.FirstOrDefault();
                            if (exampleXa != null)
                            {
                                worksheet.Cells[2, 10].Value = exampleXa.TenXa;
                            }
                        }
                    }

                    worksheet.Cells[2, 11].Value = "Số 123, đường ABC";

                    worksheet.Cells[3, 1].Value = "Trần Thị B";
                    worksheet.Cells[3, 2].Value = DateTime.Now.AddYears(-25).ToString("dd/MM/yyyy");
                    worksheet.Cells[3, 3].Value = 25;
                    worksheet.Cells[3, 4].Value = danTocs.Skip(1).FirstOrDefault()?.TenDanToc ?? "Kinh";
                    worksheet.Cells[3, 5].Value = ngheNghieps.Skip(1).FirstOrDefault()?.TenNgheNghiep ?? "Bác sĩ";
                    worksheet.Cells[3, 6].Value = "098765432109";
                    worksheet.Cells[3, 7].Value = "0987654321";

                    var fileContent = package.GetAsByteArray();
                    return File(
                        fileContent,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Employee_Template.xlsx");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể tạo file mẫu: " + ex.Message;
                return RedirectToAction("Import");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile excelFile)
        {
            ViewBag.DanTocList = new SelectList(await _employeeBusiness.GetDanTocsAsync(), "DanTocId", "TenDanToc");
            ViewBag.NgheNghiepList = new SelectList(await _employeeBusiness.GetNgheNghiepsAsync(), "NgheNghiepId", "TenNgheNghiep");
            ViewBag.TinhList = new SelectList(await _employeeBusiness.GetDanhMucTinhsAsync(), "TinhId", "TenTinh");

            List<string> errors = new List<string>();

            if (excelFile == null || excelFile.Length <= 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn tệp Excel");
                return View();
            }

            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Chỉ chấp nhận tệp Excel (.xlsx)");
                return View();
            }

            try
            {
                var allDanTocs = await _employeeBusiness.GetDanTocsAsync();
                var allNgheNghieps = await _employeeBusiness.GetNgheNghiepsAsync();
                var allTinhs = await _employeeBusiness.GetDanhMucTinhsAsync();

                var validEmployees = new List<Employee>();

                using (var stream = excelFile.OpenReadStream())
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        if (rowCount <= 1)
                        {
                            ModelState.AddModelError("", "File Excel không có dữ liệu");
                            return View();
                        }

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                string hoTen = worksheet.Cells[row, 1].Value?.ToString().Trim();
                                string ngaySinhStr = worksheet.Cells[row, 2].Value?.ToString();
                                string tuoiStr = worksheet.Cells[row, 3].Value?.ToString();
                                string danTocStr = worksheet.Cells[row, 4].Value?.ToString();
                                string ngheNghiepStr = worksheet.Cells[row, 5].Value?.ToString();
                                string cccd = worksheet.Cells[row, 6].Value?.ToString()?.Trim();
                                string sdt = worksheet.Cells[row, 7].Value?.ToString()?.Trim();
                                string tinhStr = worksheet.Cells[row, 8].Value?.ToString();
                                string huyenStr = worksheet.Cells[row, 9].Value?.ToString();
                                string xaStr = worksheet.Cells[row, 10].Value?.ToString();
                                string diaChiCuThe = worksheet.Cells[row, 11].Value?.ToString()?.Trim();

                                Employee employee = new Employee
                                {
                                    HoTen = hoTen,
                                    NgaySinh = DateOnly.TryParse(ngaySinhStr, out var ngaySinh) ? ngaySinh : null,
                                    Tuoi = int.TryParse(tuoiStr, out var tuoi) ? tuoi : null,
                                    DanTocId = allDanTocs.FirstOrDefault(d => d.TenDanToc.Equals(danTocStr, StringComparison.OrdinalIgnoreCase))?.DanTocId,
                                    NgheNghiepId = allNgheNghieps.FirstOrDefault(n => n.TenNgheNghiep.Equals(ngheNghiepStr, StringComparison.OrdinalIgnoreCase))?.NgheNghiepId,
                                    Cccd = cccd,
                                    SoDienThoai = sdt,
                                    TinhId = allTinhs.FirstOrDefault(t => t.TenTinh.Equals(tinhStr, StringComparison.OrdinalIgnoreCase))?.TinhId,
                                    HuyenId = null, 
                                    XaId = null, 
                                    DiaChiCuThe = diaChiCuThe
                                };

                                
                                if (!string.IsNullOrEmpty(huyenStr) && employee.TinhId.HasValue)
                                {
                                    var huyens = await _employeeBusiness.GetHuyenByTinhAsync(employee.TinhId.Value);
                                    employee.HuyenId = huyens.FirstOrDefault(h => h.TenHuyen.Equals(huyenStr, StringComparison.OrdinalIgnoreCase))?.HuyenId;
                                }

                                if (!string.IsNullOrEmpty(xaStr) && employee.HuyenId.HasValue)
                                {
                                    var xas = await _employeeBusiness.GetXaByHuyenAsync(employee.HuyenId.Value);
                                    employee.XaId = xas.FirstOrDefault(x => x.TenXa.Equals(xaStr, StringComparison.OrdinalIgnoreCase))?.XaId;
                                }

                                
                                ValidationResult result = await _employeeValidator.ValidateAsync(employee);
                                if (!result.IsValid)
                                {
                                    errors.Add($"Dòng {row}: {result.Errors.First().ErrorMessage}");
                                    continue;
                                }

                                
                                if (!string.IsNullOrEmpty(cccd) && validEmployees.Any(e => e.Cccd == cccd))
                                {
                                    errors.Add($"Dòng {row}: CCCD '{cccd}' đã tồn tại trong danh sách nhập");
                                    continue;
                                }

                                validEmployees.Add(employee);
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Dòng {row}: Lỗi xử lý - {ex.Message}");
                            }
                        }
                    }
                }

                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View();
                }

                foreach (var employee in validEmployees)
                {
                    await _employeeBusiness.CreateEmployeeAsync(employee);
                }

                TempData["SuccessMessage"] = $"Đã nhập thành công {validEmployees.Count} nhân viên.";
                return RedirectToAction("Index", "Employee");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi nhập file: {ex.Message}");
                return View();
            }
        }
    }
}