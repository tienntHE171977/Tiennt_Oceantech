using Tiennthe171977_Oceanteach.Models;

namespace Tiennthe171977_Oceanteach.Service
{
    public interface IEmployeeService
    {
        Task<bool> CreateEmployeeAsync(Employee employee);
        Task<List<DanhMucHuyen>> GetHuyenByTinhAsync(int tinhId);
        Task<List<DanhMucXa>> GetXaByHuyenAsync(int huyenId);
        Task<bool> IsCccdExistsAsync(string cccd);
        Task<List<DanhMucTinh>> GetDanhMucTinhsAsync();
        Task<List<DanhMucHuyen>> GetDanhMucHuyensAsync(int? tinhId);
        Task<List<DanhMucXa>> GetDanhMucXasAsync(int? huyenId);
        Task<List<DanToc>> GetDanTocsAsync();
        Task<List<NgheNghiep>> GetNgheNghiepsAsync();
        Task<bool> ValidateLocationAsync(int? tinhId, int? huyenId, int? xaId);
    }
}
