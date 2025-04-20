using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tiennthe171977_Oceanteach.Models;

namespace Tiennthe171977_Oceanteach.Service
{
    public class LocationService : ILocationService
    {
        private readonly OceantechContext _dbContext;

        public LocationService(OceantechContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        #region Tỉnh Service Operations

        public async Task<List<DanhMucTinh>> GetAllTinhAsync(string? searchTerm = null)
        {
            var query = _dbContext.DanhMucTinhs.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => t.TenTinh.Contains(searchTerm));
            }

            return await query.OrderBy(t => t.TenTinh).ToListAsync();
        }

        public async Task<DanhMucTinh?> GetTinhByIdAsync(int id)
        {
            return await _dbContext.DanhMucTinhs.FindAsync(id);
        }

        public async Task<DanhMucTinh?> GetTinhByNameAsync(string tenTinh)
        {
            return await _dbContext.DanhMucTinhs
                .FirstOrDefaultAsync(t => t.TenTinh.ToLower() == tenTinh.ToLower());
        }

        public async Task<DanhMucTinh?> GetTinhByNameExceptIdAsync(string tenTinh, int tinhId)
        {
            return await _dbContext.DanhMucTinhs
                .FirstOrDefaultAsync(t => t.TenTinh.ToLower() == tenTinh.ToLower() && t.TinhId != tinhId);
        }

        public async Task<DanhMucTinh> CreateTinhAsync(DanhMucTinh tinh)
        {
            _dbContext.DanhMucTinhs.Add(tinh);
            await _dbContext.SaveChangesAsync();
            return tinh;
        }

        public async Task<bool> UpdateTinhAsync(DanhMucTinh tinh)
        {
            var existingTinh = await _dbContext.DanhMucTinhs.FindAsync(tinh.TinhId);
            if (existingTinh == null)
                return false;

            existingTinh.TenTinh = tinh.TenTinh;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTinhAsync(int id)
        {
            var tinh = await _dbContext.DanhMucTinhs.FindAsync(id);
            if (tinh == null)
                return false;

            _dbContext.DanhMucTinhs.Remove(tinh);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion Tỉnh Service Operations

        #region Huyện Service Operations

        public async Task<List<DanhMucHuyen>> GetAllHuyenAsync(int? tinhId = null, string? searchTerm = null)
        {
            var query = _dbContext.DanhMucHuyens
                .Include(h => h.Tinh)
                .AsQueryable();

            if (tinhId.HasValue)
            {
                query = query.Where(h => h.TinhId == tinhId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(h => h.TenHuyen.Contains(searchTerm));
            }

            return await query.OrderBy(h => h.TenHuyen).ToListAsync();
        }

        public async Task<DanhMucHuyen?> GetHuyenByIdAsync(int id)
        {
            return await _dbContext.DanhMucHuyens
                .Include(h => h.Tinh)
                .FirstOrDefaultAsync(h => h.HuyenId == id);
        }

        public async Task<List<DanhMucHuyen>> GetHuyensByTinhIdAsync(int tinhId)
        {
            return await _dbContext.DanhMucHuyens
                .Where(h => h.TinhId == tinhId)
                .OrderBy(h => h.TenHuyen)
                .ToListAsync();
        }

        public async Task<DanhMucHuyen?> GetHuyenByNameAndTinhIdAsync(string tenHuyen, int tinhId)
        {
            return await _dbContext.DanhMucHuyens
                .FirstOrDefaultAsync(h =>
                    h.TenHuyen.ToLower() == tenHuyen.ToLower() &&
                    h.TinhId == tinhId);
        }

        public async Task<DanhMucHuyen?> GetHuyenByNameAndTinhIdExceptIdAsync(string tenHuyen, int tinhId, int huyenId)
        {
            return await _dbContext.DanhMucHuyens
                .FirstOrDefaultAsync(h =>
                    h.TenHuyen.ToLower() == tenHuyen.ToLower() &&
                    h.TinhId == tinhId &&
                    h.HuyenId != huyenId);
        }

        public async Task<DanhMucHuyen> CreateHuyenAsync(DanhMucHuyen huyen)
        {
            _dbContext.DanhMucHuyens.Add(huyen);
            await _dbContext.SaveChangesAsync();
            return huyen;
        }

        public async Task<bool> UpdateHuyenAsync(DanhMucHuyen huyen)
        {
            var existingHuyen = await _dbContext.DanhMucHuyens.FindAsync(huyen.HuyenId);
            if (existingHuyen == null)
                return false;

            existingHuyen.TenHuyen = huyen.TenHuyen;
            existingHuyen.TinhId = huyen.TinhId;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteHuyenAsync(int id)
        {
            var huyen = await _dbContext.DanhMucHuyens.FindAsync(id);
            if (huyen == null)
                return false;

            _dbContext.DanhMucHuyens.Remove(huyen);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion Huyện Service Operations

        #region Xã Service Operations

        public async Task<List<DanhMucXa>> GetAllXaAsync(int? huyenId = null, string? searchTerm = null)
        {
            var query = _dbContext.DanhMucXas
                .Include(x => x.Huyen)
                .ThenInclude(h => h.Tinh)
                .AsQueryable();

            if (huyenId.HasValue)
            {
                query = query.Where(x => x.HuyenId == huyenId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.TenXa.Contains(searchTerm));
            }

            return await query.OrderBy(x => x.TenXa).ToListAsync();
        }

        public async Task<DanhMucXa?> GetXaByIdAsync(int id)
        {
            return await _dbContext.DanhMucXas
                .Include(x => x.Huyen)
                .ThenInclude(h => h.Tinh)
                .FirstOrDefaultAsync(x => x.XaId == id);
        }

        public async Task<List<DanhMucXa>> GetXasByHuyenIdAsync(int huyenId)
        {
            return await _dbContext.DanhMucXas
                .Where(x => x.HuyenId == huyenId)
                .OrderBy(x => x.TenXa)
                .ToListAsync();
        }

        public async Task<DanhMucXa?> GetXaByNameAndHuyenIdAsync(string tenXa, int huyenId)
        {
            return await _dbContext.DanhMucXas
                .FirstOrDefaultAsync(x =>
                    x.TenXa.ToLower() == tenXa.ToLower() &&
                    x.HuyenId == huyenId);
        }

        public async Task<DanhMucXa?> GetXaByNameAndHuyenIdExceptIdAsync(string tenXa, int huyenId, int xaId)
        {
            return await _dbContext.DanhMucXas
                .FirstOrDefaultAsync(x =>
                    x.TenXa.ToLower() == tenXa.ToLower() &&
                    x.HuyenId == huyenId &&
                    x.XaId != xaId);
        }

        public async Task<DanhMucXa> CreateXaAsync(DanhMucXa xa)
        {
            _dbContext.DanhMucXas.Add(xa);
            await _dbContext.SaveChangesAsync();
            return xa;
        }

        public async Task<bool> UpdateXaAsync(DanhMucXa xa)
        {
            var existingXa = await _dbContext.DanhMucXas.FindAsync(xa.XaId);
            if (existingXa == null)
                return false;

            existingXa.TenXa = xa.TenXa;
            existingXa.HuyenId = xa.HuyenId;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteXaAsync(int id)
        {
            var xa = await _dbContext.DanhMucXas.FindAsync(id);
            if (xa == null)
                return false;

            _dbContext.DanhMucXas.Remove(xa);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion Xã Service Operations

        public async Task<List<Employee>> GetEmployeesByTinhIdAsync(int tinhId)
        {
            return await _dbContext.Employees
                .Where(e => e.TinhId == tinhId)
                .ToListAsync();
        }

        public async Task UpdateEmployeeLocationAsync(int employeeId, int? tinhId, int? huyenId, int? xaId)
        {
            var employee = await _dbContext.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                employee.TinhId = tinhId;
                employee.HuyenId = huyenId;
                employee.XaId = xaId;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<VanBang>> GetVanBangsByTinhIdAsync(int tinhId)
        {
            return await _dbContext.VanBangs
                .Where(v => v.DonViCap == tinhId)
                .ToListAsync();
        }

        public async Task<List<Employee>> GetEmployeesByXaIdAsync(int xaId)
        {
            return await _dbContext.Employees.Where(e => e.XaId == xaId).ToListAsync();
        }

        public async Task<List<Employee>> GetEmployeesByHuyenIdAsync(int huyenId)
        {
            return await _dbContext.Employees.Where(e => e.HuyenId == huyenId).ToListAsync();
        }

        public async Task UpdateVanBangDonViCapAsync(int vanBangId, int? newTinhId)
        {
            var vb = await _dbContext.VanBangs.FindAsync(vanBangId);
            if (vb != null)
            {
                vb.DonViCap = newTinhId;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}