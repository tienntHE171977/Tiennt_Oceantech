using Microsoft.EntityFrameworkCore;
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
    }
}
