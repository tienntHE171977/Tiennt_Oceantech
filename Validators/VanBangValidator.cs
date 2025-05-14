using FluentValidation;
using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;

namespace Tiennthe171977_Oceanteach.Validators
{
    public class VanBangValidator : BaseValidator<VanBang>
    {
        private readonly IEmployeeService _employeeService;

        public VanBangValidator(IEmployeeService employeeService)
        {
            _employeeService = employeeService;

            RuleFor(x => x.TenVanBang)
                .NotEmpty().WithMessage("Tên văn bằng không được để trống")
                .MaximumLength(200).WithMessage("Tên văn bằng không được vượt quá 200 ký tự");

            RuleFor(x => x.NgayCap)
                .NotEmpty().WithMessage("Ngày cấp không được để trống")
                .Must(BeAValidDate).WithMessage("Ngày cấp không hợp lệ");

            RuleFor(x => x.NgayHetHan)
                .GreaterThan(x => x.NgayCap).When(x => x.NgayHetHan.HasValue)
                .WithMessage("Ngày hết hạn phải sau ngày cấp");

            RuleFor(x => x.DonViCap)
                .NotNull().WithMessage("Đơn vị cấp không được để trống");

            RuleFor(x => x.EmployeeId)
                .NotNull().WithMessage("Nhân viên không được để trống")
                .MustAsync(BeValidEmployeeAsync).WithMessage("Nhân viên không tồn tại");

            
            RuleFor(x => x)
                .MustAsync(BeWithinValidVanBangLimitAsync)
                .WithMessage("Nhân viên đã có 3 văn bằng còn hạn");
        }

        private async Task<bool> BeValidEmployeeAsync(int? employeeId, CancellationToken cancellationToken)
        {
            if (!employeeId.HasValue) return false;
            return await _employeeService.GetEmployeeByIdAsync(employeeId.Value) != null;
        }

        private async Task<bool> BeWithinValidVanBangLimitAsync(VanBang vanBang, CancellationToken cancellationToken)
        {
            if (!vanBang.EmployeeId.HasValue) return false;

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var validVanBangs = await _employeeService.GetVanBangsByEmployeeIdAsync(vanBang.EmployeeId.Value);
            var validCount = validVanBangs.Count(vb =>
                vb.VanBangId != vanBang.VanBangId &&
                (!vb.NgayHetHan.HasValue || vb.NgayHetHan.Value >= currentDate));

            return validCount < 3;
        }
    }
}