using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;

namespace Tiennthe171977_Oceanteach.Business
{
    public interface IEmployeeBusiness
    {
        Task<bool> CreateEmployeeAsync(Employee employee);
        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int employeeId);
        Task<bool> AddVanBangAsync(int employeeId, VanBang vanBang);
        
        Task<bool> UpdateVanBangAsync(int employeeId, VanBang vanBang);
        Task<List<DanhMucHuyen>> GetDanhMucHuyensAsync(int? tinhId);
        Task<List<DanhMucTinh>> GetDanhMucTinhsAsync();
        Task<List<DanhMucXa>> GetDanhMucXasAsync(int? huyenId);
        Task<List<DanToc>> GetDanTocsAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<List<Employee>> GetEmployeesAsync(int page, int pageSize);
        Task<int> GetTotalEmployeesCountAsync();
        Task<List<DanhMucHuyen>> GetHuyenByTinhAsync(int tinhId);
        Task<List<NgheNghiep>> GetNgheNghiepsAsync();
        Task<List<DanhMucXa>> GetXaByHuyenAsync(int huyenId);
        
        Task<bool> ValidateLocationAsync(int? tinhId, int? huyenId, int? xaId);
        Task<bool> DeleteVanBangAsync(int employeeId, int vanBangId);
        Task<List<VanBang>> GetVanBangsByEmployeeIdAsync(int employeeId);
        Task<bool> DeleteTinhAsync(int tinhId);
        Task<List<Employee>> GetEmployeesByIdsAsync(List<int> ids);
        Task<List<Employee>> SearchEmployeesAsync(string searchTerm);
        Task<DanToc> GetDanTocByIdAsync(int id);
        Task<NgheNghiep> GetNgheNghiepByIdAsync(int id);

    }
}
