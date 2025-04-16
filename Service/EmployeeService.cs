﻿using Microsoft.EntityFrameworkCore;
using Tiennthe171977_Oceanteach.Models;

namespace Tiennthe171977_Oceanteach.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly OceantechContext _context;

        public EmployeeService(OceantechContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            // Kiểm tra trùng lặp CCCD, loại trừ CCCD của chính nhân viên đang được chỉnh sửa
            bool isCccdDuplicate = await _context.Employees
                .AnyAsync(e => e.Cccd == employee.Cccd && e.EmployeeId != employee.EmployeeId);

            if (isCccdDuplicate)
            {
                throw new Exception("CCCD đã tồn tại.");
            }

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            
            var vanBangs = _context.VanBangs.Where(v => v.EmployeeId == employeeId);
            _context.VanBangs.RemoveRange(vanBangs);

            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return false; 
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddVanBangAsync(int employeeId, VanBang vanBang)
        {
            var employee = await _context.Employees
                .Include(e => e.VanBangs)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null) return false;

            // Kiểm tra số lượng văn bằng còn hạn
            int validVanBangCount = employee.VanBangs.Count(vb => !vb.NgayHetHan.HasValue || vb.NgayHetHan > DateOnly.FromDateTime(DateTime.Now));
            if (validVanBangCount >= 3)
            {
                return false; // Không thể thêm nếu đã có 3 văn bằng còn hạn
            }

            employee.VanBangs.Add(vanBang);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteVanBangAsync(int vanBangId)
        {
            var vanBang = await _context.VanBangs.FindAsync(vanBangId);
            if (vanBang == null) return false;

            _context.VanBangs.Remove(vanBang);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<DanhMucHuyen>> GetHuyenByTinhAsync(int tinhId)
        {
            return await _context.DanhMucHuyens
                .Where(h => h.TinhId == tinhId)
                .ToListAsync();
        }

        public async Task<List<DanhMucXa>> GetXaByHuyenAsync(int huyenId)
        {
            return await _context.DanhMucXas
                .Where(x => x.HuyenId == huyenId)
                .ToListAsync();
        }

        public async Task<bool> IsCccdExistsAsync(string cccd)
        {
            return await _context.Employees.AnyAsync(e => e.Cccd == cccd);
        }

        public async Task<List<DanhMucTinh>> GetDanhMucTinhsAsync()
        {
            return await _context.DanhMucTinhs.ToListAsync();
        }

        public async Task<List<DanhMucHuyen>> GetDanhMucHuyensAsync(int? tinhId)
        {
            return await _context.DanhMucHuyens
                .Where(h => !tinhId.HasValue || h.TinhId == tinhId)
                .ToListAsync();
        }

        public async Task<List<DanhMucXa>> GetDanhMucXasAsync(int? huyenId)
        {
            return await _context.DanhMucXas
                .Where(x => !huyenId.HasValue || x.HuyenId == huyenId)
                .ToListAsync();
        }

        public async Task<List<DanToc>> GetDanTocsAsync()
        {
            return await _context.DanTocs.ToListAsync();
        }

        public async Task<List<NgheNghiep>> GetNgheNghiepsAsync()
        {
            return await _context.NgheNghieps.ToListAsync();
        }

        public async Task<bool> ValidateLocationAsync(int? tinhId, int? huyenId, int? xaId)
        {
            if (!tinhId.HasValue || !huyenId.HasValue || !xaId.HasValue) return false;

            var huyen = await _context.DanhMucHuyens
                .FirstOrDefaultAsync(h => h.HuyenId == huyenId && h.TinhId == tinhId);
            if (huyen == null) return false;

            var xa = await _context.DanhMucXas
                .FirstOrDefaultAsync(x => x.XaId == xaId && x.HuyenId == huyenId);
            return xa != null;
        }

        public async Task<bool> UpdateVanBangAsync(int employeeId, VanBang vanBang)
        {
            var existingVanBang = await _context.VanBangs.FirstOrDefaultAsync(vb => vb.VanBangId == vanBang.VanBangId && vb.EmployeeId == employeeId);
            if (existingVanBang == null) return false;

            existingVanBang.TenVanBang = vanBang.TenVanBang;
            existingVanBang.NgayCap = vanBang.NgayCap;
            existingVanBang.NgayHetHan = vanBang.NgayHetHan;
            existingVanBang.DonViCap = vanBang.DonViCap;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.DanToc)
                .Include(e => e.NgheNghiep)
                .Include(e => e.Tinh)
                .Include(e => e.Huyen)
                .Include(e => e.Xa)
                .Include(e => e.VanBangs)
                .ThenInclude(vb => vb.DonViCapNavigation)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            return employee;
        }


        public async Task<List<Employee>> GetEmployeesAsync(int page, int pageSize)
        {
            return await _context.Employees
                .OrderBy(e => e.EmployeeId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalEmployeesCountAsync()
        {
            return await _context.Employees.CountAsync();
        }
        public async Task<List<VanBang>> GetVanBangsByEmployeeIdAsync(int employeeId)
        {
            var result = await _context.VanBangs
                .Where(v => v.EmployeeId == employeeId)
                .Include(v => v.DonViCapNavigation)
                .ToListAsync();

            Console.WriteLine($"[DEBUG] Số lượng VB tìm thấy: {result.Count} cho EmployeeId = {employeeId}");

            foreach (var vb in result)
            {
                Console.WriteLine($"VB: {vb.VanBangId} - {vb.TenVanBang}");
            }

            return result;
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
    }
}
