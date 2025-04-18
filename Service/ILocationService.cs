using Microsoft.EntityFrameworkCore.Storage;
using Tiennthe171977_Oceanteach.Models;

namespace Tiennthe171977_Oceanteach.Service
{
    public interface ILocationService
    {
        Task<List<Employee>> GetEmployeesByXaIdAsync(int xaId);
        Task<List<Employee>> GetEmployeesByHuyenIdAsync(int huyenId);
        Task<List<VanBang>> GetVanBangsByTinhIdAsync(int tinhId);
        Task UpdateVanBangDonViCapAsync(int vanBangId, int? newTinhId);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<List<Employee>> GetEmployeesByTinhIdAsync(int tinhId);
        Task UpdateEmployeeLocationAsync(int employeeId, int? tinhId, int? huyenId, int? xaId);
        #region Tỉnh Service
        Task<List<DanhMucTinh>> GetAllTinhAsync(string? searchTerm = null);
        Task<DanhMucTinh?> GetTinhByIdAsync(int id);
        Task<DanhMucTinh?> GetTinhByNameAsync(string tenTinh);
        Task<DanhMucTinh?> GetTinhByNameExceptIdAsync(string tenTinh, int tinhId);
        Task<DanhMucTinh> CreateTinhAsync(DanhMucTinh tinh);
        Task<bool> UpdateTinhAsync(DanhMucTinh tinh);
        Task<bool> DeleteTinhAsync(int id);
        #endregion

        #region Huyện Service
        Task<List<DanhMucHuyen>> GetAllHuyenAsync(int? tinhId = null, string? searchTerm = null);
        Task<DanhMucHuyen?> GetHuyenByIdAsync(int id);
        Task<List<DanhMucHuyen>> GetHuyensByTinhIdAsync(int tinhId);
        Task<DanhMucHuyen?> GetHuyenByNameAndTinhIdAsync(string tenHuyen, int tinhId);
        Task<DanhMucHuyen?> GetHuyenByNameAndTinhIdExceptIdAsync(string tenHuyen, int tinhId, int huyenId);
        Task<DanhMucHuyen> CreateHuyenAsync(DanhMucHuyen huyen);
        Task<bool> UpdateHuyenAsync(DanhMucHuyen huyen);
        Task<bool> DeleteHuyenAsync(int id);
        #endregion

        #region Xã Service
        Task<List<DanhMucXa>> GetAllXaAsync(int? huyenId = null, string? searchTerm = null);
        Task<DanhMucXa?> GetXaByIdAsync(int id);
        Task<List<DanhMucXa>> GetXasByHuyenIdAsync(int huyenId);
        Task<DanhMucXa?> GetXaByNameAndHuyenIdAsync(string tenXa, int huyenId);
        Task<DanhMucXa?> GetXaByNameAndHuyenIdExceptIdAsync(string tenXa, int huyenId, int xaId);
        Task<DanhMucXa> CreateXaAsync(DanhMucXa xa);
        Task<bool> UpdateXaAsync(DanhMucXa xa);
        Task<bool> DeleteXaAsync(int id);
        
        #endregion

    }
}
