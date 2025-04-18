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
        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int employeeId);
        
        Task<Employee?> GetEmployeeByIdAsync(int id); 
        Task<List<Employee>> GetEmployeesAsync(int page, int pageSize); 
        Task<int> GetTotalEmployeesCountAsync();
        Task<bool> AddVanBangAsync(int employeeId, VanBang vanBang);
        Task<bool> DeleteVanBangAsync(int employeeId, int vanBangId);
        Task<List<VanBang>> GetVanBangsByEmployeeIdAsync(int employeeId);
        Task<bool> UpdateVanBangAsync(int employeeId, VanBang vanBang);
        Task<List<Employee>> GetEmployeesByIdsAsync(List<int> ids);
        Task<List<Employee>> SearchEmployeesAsync(string searchTerm);
        Task<DanToc> GetDanTocByIdAsync(int id);
        Task<NgheNghiep> GetNgheNghiepByIdAsync(int id);
    }
}
