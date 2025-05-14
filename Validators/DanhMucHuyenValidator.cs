using FluentValidation;
using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;

namespace Tiennthe171977_Oceanteach.Validators
{
    public class DanhMucHuyenValidator : BaseValidator<DanhMucHuyen>
    {
        private readonly ILocationService _locationService;

        public DanhMucHuyenValidator(ILocationService locationService)
        {
            _locationService = locationService;

            RuleFor(x => x.TenHuyen)
                .NotEmpty().WithMessage("Tên huyện không được để trống")
                .MaximumLength(100).WithMessage("Tên huyện không được vượt quá 100 ký tự")
                .MustAsync(BeUniqueInTinhAsync).WithMessage("Tên huyện đã tồn tại trong tỉnh này");

            RuleFor(x => x.TinhId)
                .NotNull().WithMessage("Tỉnh không được để trống")
                .MustAsync(BeValidTinhAsync).WithMessage("Tỉnh không tồn tại");
        }

        private async Task<bool> BeUniqueInTinhAsync(DanhMucHuyen huyen, string tenHuyen, CancellationToken cancellationToken)
        {
            if (!huyen.TinhId.HasValue) return false;
            var existingHuyen = await _locationService.GetHuyenByNameAndTinhIdExceptIdAsync(tenHuyen, huyen.TinhId.Value, huyen.HuyenId);
            return existingHuyen == null;
        }

        private async Task<bool> BeValidTinhAsync(int? tinhId, CancellationToken cancellationToken)
        {
            if (!tinhId.HasValue) return false;
            return await _locationService.GetTinhByIdAsync(tinhId.Value) != null;
        }
    }
}