using FluentValidation;
using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;

namespace Tiennthe171977_Oceanteach.Validators
{
    public class EmployeeValidator : BaseValidator<Employee>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeValidator(IEmployeeService employeeService)
        {
            _employeeService = employeeService;

            RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage("Họ tên không được để trống")
                .MaximumLength(100).WithMessage("Họ tên không được vượt quá 100 ký tự");

            RuleFor(x => x)
            .Must(e =>
    {
            if (e.NgaySinh.HasValue && e.Tuoi.HasValue)
            {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - e.NgaySinh.Value.Year;
            if (e.NgaySinh.Value > today.AddYears(-age)) age--;
            return age == e.Tuoi.Value;
            }
            return true;
                })
            .WithMessage("Tuổi không khớp với ngày sinh.");

            RuleFor(x => x.Tuoi)
                .Must(BeAValidAge).WithMessage("Tuổi phải từ 18 đến 100");

            RuleFor(x => x.SoDienThoai)
                .Must(soDienThoai => string.IsNullOrEmpty(soDienThoai) || BeAValidPhoneNumber(soDienThoai))
                .WithMessage("Số điện thoại không hợp lệ");

            RuleFor(x => x.Cccd)
                .Must(cccd => string.IsNullOrEmpty(cccd) || BeAValidCCCD(cccd))
                .WithMessage("CCCD phải là 12 chữ số")
                .MustAsync(BeUniqueAsync).When(x => !string.IsNullOrEmpty(x.Cccd))
                .WithMessage("CCCD đã tồn tại");

            RuleFor(x => x.DanTocId)
                .NotNull().WithMessage("Dân tộc không được để trống")
                .MustAsync(BeValidDanTocAsync).WithMessage("Dân tộc không tồn tại");

            RuleFor(x => x.NgheNghiepId)
                .NotNull().WithMessage("Nghề nghiệp không được để trống")
                .MustAsync(BeValidNgheNghiepAsync).WithMessage("Nghề nghiệp không tồn tại");

            RuleFor(x => x)
                .MustAsync(BeValidLocationAsync).WithMessage("Thông tin tỉnh, huyện, xã không hợp lệ");
        }

        private async Task<bool> BeUniqueAsync(Employee employee, string cccd, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(cccd)) return true;
            var exists = await _employeeService.IsCccdExistsAsync(cccd);
            if (exists)
            {
                var currentEmployee = await _employeeService.GetEmployeeByIdAsync(employee.EmployeeId);
                return currentEmployee != null && currentEmployee.Cccd == cccd;
            }
            return true;
        }

        private async Task<bool> BeValidDanTocAsync(int? danTocId, CancellationToken cancellationToken)
        {
            if (!danTocId.HasValue) return false;
            var danToc = await _employeeService.GetDanTocByIdAsync(danTocId.Value);
            return danToc != null;
        }

        private async Task<bool> BeValidNgheNghiepAsync(int? ngheNghiepId, CancellationToken cancellationToken)
        {
            if (!ngheNghiepId.HasValue) return false;
            var ngheNghiep = await _employeeService.GetNgheNghiepByIdAsync(ngheNghiepId.Value);
            return ngheNghiep != null;
        }

        private async Task<bool> BeValidLocationAsync(Employee employee, CancellationToken cancellationToken)
        {
            if (!employee.TinhId.HasValue || !employee.HuyenId.HasValue || !employee.XaId.HasValue)
                return false;
            return await _employeeService.ValidateLocationAsync(employee.TinhId, employee.HuyenId, employee.XaId);
        }
    }
}