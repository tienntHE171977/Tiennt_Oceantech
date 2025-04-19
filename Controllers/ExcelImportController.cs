using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using Tiennthe171977_Oceanteach.Business;
using Tiennthe171977_Oceanteach.Models;

namespace Tiennthe171977_Oceanteach.Controllers
{
    public class ExcelImportController : Controller
    {
        private readonly IEmployeeBusiness _employeeBusiness;

        public ExcelImportController(IEmployeeBusiness employeeBusiness)
        {
            _employeeBusiness = employeeBusiness;
        }

        // GET: ExcelImport/Import
        public async Task<IActionResult> Import()
        {
            // Load data for select lists
            ViewBag.DanTocList = new SelectList(await _employeeBusiness.GetDanTocsAsync(), "DanTocId", "TenDanToc");
            ViewBag.NgheNghiepList = new SelectList(await _employeeBusiness.GetNgheNghiepsAsync(), "NgheNghiepId", "TenNgheNghiep");
            ViewBag.TinhList = new SelectList(await _employeeBusiness.GetDanhMucTinhsAsync(), "TinhId", "TenTinh");

            return View();
        }
        // GET: ExcelImport/DownloadTemplate
        public async Task<IActionResult> DownloadTemplate()
        {
            try
            {
                // Create a new Excel package
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    // Add a new worksheet to the empty workbook
                    var worksheet = package.Workbook.Worksheets.Add("Employees");

                    // Define and style header row
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

                    // Auto-fit columns
                    for (int i = 1; i <= headers.Count; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    // Load sample data for dropdowns
                    var danTocs = await _employeeBusiness.GetDanTocsAsync();
                    var ngheNghieps = await _employeeBusiness.GetNgheNghiepsAsync();
                    var tinhs = await _employeeBusiness.GetDanhMucTinhsAsync();

                    // Add some example data
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

                    // Create a second example row
                    worksheet.Cells[3, 1].Value = "Trần Thị B";
                    worksheet.Cells[3, 2].Value = DateTime.Now.AddYears(-25).ToString("dd/MM/yyyy");
                    worksheet.Cells[3, 3].Value = 25;
                    worksheet.Cells[3, 4].Value = danTocs.Skip(1).FirstOrDefault()?.TenDanToc ?? "Kinh";
                    worksheet.Cells[3, 5].Value = ngheNghieps.Skip(1).FirstOrDefault()?.TenNgheNghiep ?? "Bác sĩ";
                    worksheet.Cells[3, 6].Value = "098765432109";
                    worksheet.Cells[3, 7].Value = "0987654321";

                    // Convert to byte array
                    var fileContent = package.GetAsByteArray();

                    // Return the Excel file with specific content-disposition to force download
                    return File(
                        fileContent,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Employee_Template.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Log error and store in TempData
                TempData["Error"] = "Không thể tạo file mẫu: " + ex.Message;
                return RedirectToAction("Import");
            }
        }

        // POST: ExcelImport/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile excelFile)
        {
            // Load lookups for validation and view
            ViewBag.DanTocList = new SelectList(await _employeeBusiness.GetDanTocsAsync(), "DanTocId", "TenDanToc");
            ViewBag.NgheNghiepList = new SelectList(await _employeeBusiness.GetNgheNghiepsAsync(), "NgheNghiepId", "TenNgheNghiep");
            ViewBag.TinhList = new SelectList(await _employeeBusiness.GetDanhMucTinhsAsync(), "TinhId", "TenTinh");

            // Validation messages
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
                // Load lookups for data validation
                var allDanTocs = await _employeeBusiness.GetDanTocsAsync();
                var allNgheNghieps = await _employeeBusiness.GetNgheNghiepsAsync();
                var allTinhs = await _employeeBusiness.GetDanhMucTinhsAsync();

                var validEmployees = new List<Employee>();

                using (var stream = excelFile.OpenReadStream())
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // First worksheet
                        int rowCount = worksheet.Dimension.Rows;

                        // Check if file has data
                        if (rowCount <= 1) // Only header row or empty
                        {
                            ModelState.AddModelError("", "File Excel không có dữ liệu");
                            return View();
                        }

                        // Process each row (starting from row 2, after header)
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                // Extract raw values from Excel
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

                                // Validate required fields
                                if (string.IsNullOrEmpty(hoTen))
                                {
                                    errors.Add($"Dòng {row}: Họ tên không được để trống");
                                    continue;
                                }

                                // Parse and validate date
                                DateTime ngaySinh;
                                if (!DateTime.TryParse(ngaySinhStr, out ngaySinh))
                                {
                                    errors.Add($"Dòng {row}: Ngày sinh không hợp lệ");
                                    continue;
                                }

                                // Check if date is not in future
                                if (ngaySinh > DateTime.Now)
                                {
                                    errors.Add($"Dòng {row}: Ngày sinh không được là ngày trong tương lai");
                                    continue;
                                }

                                // Parse and validate tuoi
                                int tuoi;
                                if (!int.TryParse(tuoiStr, out tuoi) || tuoi < 18 || tuoi > 100)
                                {
                                    errors.Add($"Dòng {row}: Tuổi phải là số và trong khoảng 18-100");
                                    continue;
                                }

                                // Validate CCCD if provided
                                if (!string.IsNullOrEmpty(cccd) && !System.Text.RegularExpressions.Regex.IsMatch(cccd, @"^\d{12}$"))
                                {
                                    errors.Add($"Dòng {row}: CCCD phải là 12 chữ số");
                                    continue;
                                }

                                // Validate SĐT if provided
                                if (!string.IsNullOrEmpty(sdt) && !System.Text.RegularExpressions.Regex.IsMatch(sdt, @"^0[3579]\d{8}$"))
                                {
                                    errors.Add($"Dòng {row}: Số điện thoại không hợp lệ");
                                    continue;
                                }

                                // Find DanToc by name
                                var danToc = allDanTocs.FirstOrDefault(d => d.TenDanToc.Equals(danTocStr, StringComparison.OrdinalIgnoreCase));
                                if (danToc == null)
                                {
                                    errors.Add($"Dòng {row}: Dân tộc '{danTocStr}' không tồn tại trong hệ thống");
                                    continue;
                                }

                                // Find NgheNghiep by name
                                var ngheNghiep = allNgheNghieps.FirstOrDefault(n => n.TenNgheNghiep.Equals(ngheNghiepStr, StringComparison.OrdinalIgnoreCase));
                                if (ngheNghiep == null)
                                {
                                    errors.Add($"Dòng {row}: Nghề nghiệp '{ngheNghiepStr}' không tồn tại trong hệ thống");
                                    continue;
                                }

                                // Find Tinh by name
                                var tinh = allTinhs.FirstOrDefault(t => t.TenTinh.Equals(tinhStr, StringComparison.OrdinalIgnoreCase));
                                if (tinh == null)
                                {
                                    errors.Add($"Dòng {row}: Tỉnh/thành phố '{tinhStr}' không tồn tại trong hệ thống");
                                    continue;
                                }

                                // Get associated Huyen for this Tinh
                                var huyens = await _employeeBusiness.GetHuyenByTinhAsync(tinh.TinhId);
                                var huyen = huyens.FirstOrDefault(h => h.TenHuyen.Equals(huyenStr, StringComparison.OrdinalIgnoreCase));
                                if (huyen == null)
                                {
                                    errors.Add($"Dòng {row}: Quận/huyện '{huyenStr}' không tồn tại trong tỉnh '{tinhStr}'");
                                    continue;
                                }

                                // Get associated Xa for this Huyen
                                var xas = await _employeeBusiness.GetXaByHuyenAsync(huyen.HuyenId);
                                var xa = xas.FirstOrDefault(x => x.TenXa.Equals(xaStr, StringComparison.OrdinalIgnoreCase));
                                if (xa == null)
                                {
                                    errors.Add($"Dòng {row}: Xã/phường '{xaStr}' không tồn tại trong quận/huyện '{huyenStr}'");
                                    continue;
                                }

                                // Validate location relationships
                                if (!await _employeeBusiness.ValidateLocationAsync(tinh.TinhId, huyen.HuyenId, xa.XaId))
                                {
                                    errors.Add($"Dòng {row}: Thông tin tỉnh, huyện, xã không hợp lệ");
                                    continue;
                                }

                                // Validate CCCD duplicate
                                if (!string.IsNullOrEmpty(cccd))
                                {
                                    // Check in database for duplicate CCCD
                                    bool cccdExists = await _employeeBusiness.IsCccdExistsAsync(cccd);
                                    // Also check in the current import batch
                                    bool duplicateInBatch = validEmployees.Any(e => e.Cccd == cccd);

                                    if (cccdExists || duplicateInBatch)
                                    {
                                        errors.Add($"Dòng {row}: CCCD '{cccd}' đã tồn tại");
                                        continue;
                                    }
                                }

                                // If we got here, the row data is valid, create Employee object
                                Employee employee = new Employee
                                {
                                    HoTen = hoTen,
                                    NgaySinh = DateOnly.FromDateTime(ngaySinh),
                                    Tuoi = tuoi,
                                    DanTocId = danToc.DanTocId,
                                    NgheNghiepId = ngheNghiep.NgheNghiepId,
                                    Cccd = cccd,
                                    SoDienThoai = sdt,
                                    TinhId = tinh.TinhId,
                                    HuyenId = huyen.HuyenId,
                                    XaId = xa.XaId,
                                    DiaChiCuThe = diaChiCuThe
                                };

                                validEmployees.Add(employee);
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Dòng {row}: Lỗi xử lý - {ex.Message}");
                            }
                        }
                    }
                }

                // If there are any errors, show them and don't save
                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View();
                }

                // Save valid employees to the database
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
