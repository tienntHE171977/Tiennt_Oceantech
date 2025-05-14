using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;

namespace Tiennthe171977_Oceanteach.Business
{
    public class EmployeeBusiness : IEmployeeBusiness
    {
        private readonly OceantechContext _context;
        private readonly IEmployeeService _employeeService;
        private readonly IValidator<Employee> _employeeValidator;
        private readonly IValidator<VanBang> _vanBangValidator;

        public EmployeeBusiness(
            IEmployeeService employeeService,
            OceantechContext context,
            IValidator<Employee> employeeValidator,
            IValidator<VanBang> vanBangValidator)
        {
            _employeeService = employeeService;
            _context = context;
            _employeeValidator = employeeValidator;
            _vanBangValidator = vanBangValidator;
        }

        public async Task<bool> CreateEmployeeAsync(Employee employee)
        {
            FluentValidation.Results.ValidationResult result = await _employeeValidator.ValidateAsync(employee);
            if (!result.IsValid)
            {
                throw new FluentValidation.ValidationException(result.Errors.First().ErrorMessage);
            }

            return await _employeeService.CreateEmployeeAsync(employee);
        }

        public async Task<bool> IsCccdExistsAsync(string cccd)
        {
            return await _employeeService.IsCccdExistsAsync(cccd);
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            FluentValidation.Results.ValidationResult result = await _employeeValidator.ValidateAsync(employee);
            if (!result.IsValid)
            {
                throw new FluentValidation.ValidationException(result.Errors.First().ErrorMessage);
            }

            return await _employeeService.UpdateEmployeeAsync(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            return await _employeeService.DeleteEmployeeAsync(employeeId);
        }

        public async Task<bool> AddVanBangAsync(int employeeId, VanBang vanBang)
        {
            vanBang.EmployeeId = employeeId;
            FluentValidation.Results.ValidationResult result = await _vanBangValidator.ValidateAsync(vanBang);
            if (!result.IsValid)
            {
                throw new FluentValidation.ValidationException(result.Errors.First().ErrorMessage);
            }

            return await _employeeService.AddVanBangAsync(employeeId, vanBang);
        }

        public async Task<bool> UpdateVanBangAsync(int employeeId, VanBang vanBang)
        {
            vanBang.EmployeeId = employeeId;
            FluentValidation.Results.ValidationResult result = await _vanBangValidator.ValidateAsync(vanBang);
            if (!result.IsValid)
            {
                throw new FluentValidation.ValidationException(result.Errors.First().ErrorMessage);
            }

            return await _employeeService.UpdateVanBangAsync(employeeId, vanBang);
        }

        public async Task<bool> DeleteVanBangAsync(int employeeId, int vanBangId)
        {
            return await _employeeService.DeleteVanBangAsync(employeeId, vanBangId);
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
            return await _employeeService.DeleteTinhAsync(tinhId);
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
            return await _employeeService.ValidateLocationAsync(tinhId, huyenId, xaId);
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