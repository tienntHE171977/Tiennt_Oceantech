using FluentValidation;
using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;

namespace Tiennthe171977_Oceanteach.Validators
{
    public class DanhMucXaValidator : BaseValidator<DanhMucXa>
    {
        private readonly ILocationService _locationService;

        public DanhMucXaValidator(ILocationService locationService)
        {
            _locationService = locationService;

            RuleFor(x => x.TenXa)
                .NotEmpty().WithMessage("Tên xã không được để trống")
                .MaximumLength(100).WithMessage("Tên xã không được vượt quá 100 ký tự")
                .MustAsync(BeUniqueInHuyenAsync).WithMessage("Tên xã đã tồn tại trong huyện này");

            RuleFor(x => x.HuyenId)
                .NotNull().WithMessage("Huyện không được để trống")
                .MustAsync(BeValidHuyenAsync).WithMessage("Huyện không tồn tại");
        }

        private async Task<bool> BeUniqueInHuyenAsync(DanhMucXa xa, string tenXa, CancellationToken cancellationToken)
        {
            if (!xa.HuyenId.HasValue) return false;
            var existingXa = await _locationService.GetXaByNameAndHuyenIdExceptIdAsync(tenXa, xa.HuyenId.Value, xa.XaId);
            return existingXa == null;
        }

        private async Task<bool> BeValidHuyenAsync(int? huyenId, CancellationToken cancellationToken)
        {
            if (!huyenId.HasValue) return false;
            return await _locationService.GetHuyenByIdAsync(huyenId.Value) != null;
        }
    }
}