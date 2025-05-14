using FluentValidation;
using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;

namespace Tiennthe171977_Oceanteach.Validators
{
    public class DanhMucTinhValidator : BaseValidator<DanhMucTinh>
    {
        private readonly ILocationService _locationService;

        public DanhMucTinhValidator(ILocationService locationService)
        {
            _locationService = locationService;

            RuleFor(x => x.TenTinh)
                .NotEmpty().WithMessage("Tên tỉnh không được để trống")
                .MaximumLength(100).WithMessage("Tên tỉnh không được vượt quá 100 ký tự")
                .MustAsync(BeUniqueAsync).WithMessage("Tên tỉnh đã tồn tại");
        }

        private async Task<bool> BeUniqueAsync(DanhMucTinh tinh, string tenTinh, CancellationToken cancellationToken)
        {
            var existingTinh = await _locationService.GetTinhByNameExceptIdAsync(tenTinh, tinh.TinhId);
            return existingTinh == null;
        }
    }
}
