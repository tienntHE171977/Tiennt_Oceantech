using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;

namespace Tiennthe171977_Oceanteach.Business
{
    public class EmployeeBusiness : IEmployeeBusiness
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeBusiness(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
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
    }
}