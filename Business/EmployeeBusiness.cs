using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;
using System.Linq;
using System.Globalization;
using System;

namespace Tiennthe171977_Oceanteach.Business
{
    public class EmployeeBusiness : IEmployeeBusiness
    {
        private readonly OceantechContext _context;
        private readonly IEmployeeService _employeeService;

        public EmployeeBusiness(IEmployeeService employeeService, OceantechContext context)
        {
            _employeeService = employeeService;
            _context = context;
        }

        public async Task<bool> CreateEmployeeAsync(Employee employee)
        {
            // Validate mối quan hệ tỉnh-huyện-xã
            if (!await _employeeService.ValidateLocationAsync(employee.TinhId, employee.HuyenId, employee.XaId))
            {
                throw new Exception("Thông tin tỉnh, huyện, xã không hợp lệ.");
            }

            // Kiểm tra CCCD trùng lặp
            if (!string.IsNullOrEmpty(employee.Cccd) && await _employeeService.IsCccdExistsAsync(employee.Cccd))
            {
                throw new Exception("CCCD Đã Tồn Tại.");
            }

            return await _employeeService.CreateEmployeeAsync(employee);
        }
        public async Task<bool> IsCccdExistsAsync(string cccd)
        {
            if (string.IsNullOrEmpty(cccd))
                return false;

            return await _employeeService.IsCccdExistsAsync(cccd);
        }
        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            // Validate mối quan hệ tỉnh-huyện-xã
            if (!await _employeeService.ValidateLocationAsync(employee.TinhId, employee.HuyenId, employee.XaId))
            {
                throw new Exception("Thông tin tỉnh, huyện, xã không hợp lệ.");
            }

            // Kiểm tra CCCD trùng lặp, loại trừ CCCD của chính nhân viên đang được chỉnh sửa
            if (!string.IsNullOrEmpty(employee.Cccd))
            {
                bool isCccdDuplicate = await _context.Employees
                    .AnyAsync(e => e.Cccd == employee.Cccd && e.EmployeeId != employee.EmployeeId);

                if (isCccdDuplicate)
                {
                    throw new Exception("CCCD Đã Tồn Tại.");
                }
            }

            return await _employeeService.UpdateEmployeeAsync(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            return await _employeeService.DeleteEmployeeAsync(employeeId);
        }
        public async Task<bool> AddVanBangAsync(int employeeId, VanBang vanBang)
        {
            try
            {
                // Kiểm tra điều kiện
                if (string.IsNullOrEmpty(vanBang.TenVanBang))
                {
                    throw new ValidationException("Tên văn bằng không được để trống.");
                }

                if (vanBang.NgayHetHan.HasValue && vanBang.NgayHetHan <= vanBang.NgayCap)
                {
                    throw new ValidationException("Ngày hết hạn phải sau ngày cấp.");
                }

                if (!vanBang.DonViCap.HasValue)
                {
                    throw new ValidationException("Đơn vị cấp không được để trống.");
                }
                var currentDate = DateOnly.FromDateTime(DateTime.Now);

                // Kiểm tra số lượng văn bằng còn hiệu lực
                var validVanBangs = await _context.VanBangs
                    .Where(vb => vb.EmployeeId == employeeId)
                    .Where(vb =>
                        // Văn bằng không có ngày hết hạn 
                        (vb.NgayHetHan == null) ||
                        // Hoặc văn bằng còn hiệu lực (ngày hết hạn lớn hơn hoặc bằng ngày hiện tại)
                        (vb.NgayHetHan.HasValue && vb.NgayHetHan.Value >= currentDate))
                    .ToListAsync();

                // Giới hạn tối đa 3 văn bằng còn hiệu lực
                if (validVanBangs.Count >= 3)
                {
                    throw new ValidationException("Nhân viên đã có 3 văn bằng còn hạn.");
                }

                // Thêm văn bằng
                return await _employeeService.AddVanBangAsync(employeeId, vanBang);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<bool> UpdateVanBangAsync(int employeeId, VanBang vanBang)
        {
            try
            {
                // Validate dữ liệu
                if (string.IsNullOrEmpty(vanBang.TenVanBang))
                {
                    throw new Exception("Tên văn bằng không được để trống.");
                }

                //if (vanBang.NgayHetHan.HasValue && vanBang.NgayHetHan <= DateOnly.FromDateTime(DateTime.Now))
                //{
                //    throw new Exception("Ngày hết hạn phải lớn hơn ngày hiện tại.");
                //}

                if (vanBang.NgayHetHan.HasValue && vanBang.NgayHetHan <= vanBang.NgayCap)
                {
                    throw new Exception("Ngày hết hạn phải sau ngày cấp.");
                }

                if (!vanBang.DonViCap.HasValue)
                {
                    throw new Exception("Đơn vị cấp không được để trống.");
                }

                return await _employeeService.UpdateVanBangAsync(employeeId, vanBang);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> DeleteVanBangAsync(int employeeId, int vanBangId)
        {
            var vanBang = await _context.VanBangs
                .FirstOrDefaultAsync(v => v.VanBangId == vanBangId && v.EmployeeId == employeeId);

            if (vanBang == null)
            {
                return false; // Không tìm thấy văn bằng
            }

            _context.VanBangs.Remove(vanBang);
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<List<DanhMucHuyen>> GetDanhMucHuyensAsync(int? tinhId)
        {
            return await _employeeService.GetDanhMucHuyensAsync(tinhId);
        }

        public async Task<List<DanhMucTinh>> GetDanhMucTinhsAsync()
        {
            return await _employeeService.GetDanhMucTinhsAsync();
        }

        public async Task<List<DanhMucXa>> GetDanhMucXasAsync(int? huyenId)
        {
            return await _employeeService.GetDanhMucXasAsync(huyenId);
        }

        public async Task<List<DanToc>> GetDanTocsAsync()
        {
            return await _employeeService.GetDanTocsAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _employeeService.GetEmployeeByIdAsync(id);
        }

        public async Task<List<Employee>> GetEmployeesAsync(int page, int pageSize)
        {
            return await _employeeService.GetEmployeesAsync(page, pageSize);
        }

        public async Task<int> GetTotalEmployeesCountAsync()
        {
            return await _employeeService.GetTotalEmployeesCountAsync();
        }

        

        public async Task<List<NgheNghiep>> GetNgheNghiepsAsync()
        {
            return await _employeeService.GetNgheNghiepsAsync();
        }

        
        public async Task<List<VanBang>> GetVanBangsByEmployeeIdAsync(int employeeId)
        {
            return await _employeeService.GetVanBangsByEmployeeIdAsync(employeeId);
        }
        public async Task<bool> DeleteTinhAsync(int tinhId)
        {
            var huyenList = await _context.DanhMucHuyens.Where(h => h.TinhId == tinhId).ToListAsync();
            foreach (var huyen in huyenList)
            {
                var xaList = await _context.DanhMucXas.Where(x => x.HuyenId == huyen.HuyenId).ToListAsync();
                _context.DanhMucXas.RemoveRange(xaList);
            }
            _context.DanhMucHuyens.RemoveRange(huyenList);

            var tinh = await _context.DanhMucTinhs.FindAsync(tinhId);
            if (tinh == null) return false;

            _context.DanhMucTinhs.Remove(tinh);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<DanhMucHuyen>> GetHuyenByTinhAsync(int tinhId)
        {
            return await _context.DanhMucHuyens.Where(h => h.TinhId == tinhId).ToListAsync();
        }

        public async Task<List<DanhMucXa>> GetXaByHuyenAsync(int huyenId)
        {
            return await _context.DanhMucXas.Where(x => x.HuyenId == huyenId).ToListAsync();
        }
        public async Task<bool> ValidateLocationAsync(int? tinhId, int? huyenId, int? xaId)
        {
            if (!tinhId.HasValue || !huyenId.HasValue || !xaId.HasValue)
            {
                return false; // Nếu bất kỳ giá trị nào không có, trả về false
            }

            // Kiểm tra Huyện thuộc Tỉnh
            var huyen = await _context.DanhMucHuyens
                .FirstOrDefaultAsync(h => h.HuyenId == huyenId && h.TinhId == tinhId);
            if (huyen == null)
            {
                return false; // Huyện không thuộc Tỉnh
            }

            // Kiểm tra Xã thuộc Huyện
            var xa = await _context.DanhMucXas
                .FirstOrDefaultAsync(x => x.XaId == xaId && x.HuyenId == huyenId);
            return xa != null; // Trả về true nếu Xã thuộc Huyện, ngược lại false
        }
        public async Task<List<Employee>> GetEmployeesByIdsAsync(List<int> ids)
        {
            return await _employeeService.GetEmployeesByIdsAsync(ids);
        }

        public async Task<List<Employee>> SearchEmployeesAsync(string searchTerm)
        {
            return await _employeeService.SearchEmployeesAsync(searchTerm);
        }

        public async Task<DanToc> GetDanTocByIdAsync(int id)
        {
            return await _employeeService.GetDanTocByIdAsync(id);
        }

        public async Task<NgheNghiep> GetNgheNghiepByIdAsync(int id)
        {
            return await _employeeService.GetNgheNghiepByIdAsync(id);
        }
    }
}
