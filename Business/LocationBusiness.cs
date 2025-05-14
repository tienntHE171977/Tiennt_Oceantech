using FluentValidation;
using FluentValidation.Results;
using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;

namespace Tiennthe171977_Oceanteach.Business
{
    public class LocationBusiness : ILocationBusiness
    {
        private readonly ILocationService _locationService;
        private readonly IValidator<DanhMucTinh> _tinhValidator;
        private readonly IValidator<DanhMucHuyen> _huyenValidator;
        private readonly IValidator<DanhMucXa> _xaValidator;

        public LocationBusiness(
            ILocationService locationService,
            IValidator<DanhMucTinh> tinhValidator,
            IValidator<DanhMucHuyen> huyenValidator,
            IValidator<DanhMucXa> xaValidator)
        {
            _locationService = locationService;
            _tinhValidator = tinhValidator;
            _huyenValidator = huyenValidator;
            _xaValidator = xaValidator;
        }

        public async Task<List<DanhMucTinh>> GetAllTinhAsync(string? searchTerm = null)
        {
            return await _locationService.GetAllTinhAsync(searchTerm);
        }

        public async Task<DanhMucTinh?> GetTinhByIdAsync(int id)
        {
            return await _locationService.GetTinhByIdAsync(id);
        }

        public async Task<DanhMucTinh> CreateTinhAsync(DanhMucTinh tinh)
        {
            ValidationResult result = await _tinhValidator.ValidateAsync(tinh);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors.First().ErrorMessage);
            }

            return await _locationService.CreateTinhAsync(tinh);
        }

        public async Task<bool> UpdateTinhAsync(DanhMucTinh tinh)
        {
            ValidationResult result = await _tinhValidator.ValidateAsync(tinh);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors.First().ErrorMessage);
            }

            return await _locationService.UpdateTinhAsync(tinh);
        }

        public async Task<bool> DeleteTinhAsync(int id)
        {
            return await _locationService.DeleteTinhAsync(id);
        }

        public async Task<bool> DeleteTinhWithDependenciesAsync(int tinhId)
        {
            using var transaction = await _locationService.BeginTransactionAsync();
            try
            {
                await HandleEmployeesReferencingTinhAsync(tinhId);
                await HandleVanBangReferencingTinhAsync(tinhId);

                var huyens = await GetHuyensByTinhIdAsync(tinhId);
                foreach (var huyen in huyens)
                {
                    await DeleteHuyenWithDependenciesAsync(huyen.HuyenId);
                }

                var result = await _locationService.DeleteTinhAsync(tinhId);
                if (result)
                {
                    await transaction.CommitAsync();
                    return true;
                }

                await transaction.RollbackAsync();
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Lỗi khi xóa tỉnh: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        private async Task HandleEmployeesReferencingTinhAsync(int tinhId)
        {
            var employees = await _locationService.GetEmployeesByTinhIdAsync(tinhId);
            foreach (var employee in employees)
            {
                await _locationService.UpdateEmployeeLocationAsync(employee.EmployeeId, null, null, null);
            }
        }

        public async Task<bool> DeleteHuyenWithDependenciesAsync(int huyenId)
        {
            try
            {
                await HandleEmployeesReferencingHuyenAsync(huyenId);

                var xas = await GetXasByHuyenIdAsync(huyenId);
                foreach (var xa in xas)
                {
                    await HandleEmployeesReferencingXaAsync(xa.XaId);
                }

                foreach (var xa in xas)
                {
                    var result = await _locationService.DeleteXaAsync(xa.XaId);
                    if (!result)
                    {
                        return false;
                    }
                }

                var deleteResult = await _locationService.DeleteHuyenAsync(huyenId);
                return deleteResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa huyện: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task<List<dynamic>> GetAllHuyenAsync(int? tinhId = null, string? searchTerm = null)
        {
            var huyens = await _locationService.GetAllHuyenAsync(tinhId, searchTerm);
            var result = new List<dynamic>();

            foreach (var huyen in huyens)
            {
                var tinh = await _locationService.GetTinhByIdAsync(huyen.TinhId ?? 0);
                result.Add(new
                {
                    huyen.HuyenId,
                    huyen.TenHuyen,
                    huyen.TinhId,
                    TenTinh = tinh?.TenTinh ?? "Không xác định"
                });
            }

            return result;
        }

        public async Task<dynamic?> GetHuyenByIdAsync(int id)
        {
            var huyen = await _locationService.GetHuyenByIdAsync(id);
            if (huyen == null)
                return null;

            var tinh = await _locationService.GetTinhByIdAsync(huyen.TinhId ?? 0);
            return new
            {
                huyen.HuyenId,
                huyen.TenHuyen,
                huyen.TinhId,
                TenTinh = tinh?.TenTinh ?? "Không xác định"
            };
        }

        public async Task<List<DanhMucHuyen>> GetHuyensByTinhIdAsync(int tinhId)
        {
            return await _locationService.GetHuyensByTinhIdAsync(tinhId);
        }

        public async Task<DanhMucHuyen> CreateHuyenAsync(DanhMucHuyen huyen)
        {
            ValidationResult result = await _huyenValidator.ValidateAsync(huyen);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors.First().ErrorMessage);
            }

            return await _locationService.CreateHuyenAsync(huyen);
        }

        public async Task<bool> UpdateHuyenAsync(DanhMucHuyen huyen)
        {
            ValidationResult result = await _huyenValidator.ValidateAsync(huyen);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors.First().ErrorMessage);
            }

            return await _locationService.UpdateHuyenAsync(huyen);
        }

        public async Task<bool> DeleteHuyenAsync(int id)
        {
            return await _locationService.DeleteHuyenAsync(id);
        }

        private async Task HandleVanBangReferencingTinhAsync(int tinhId)
        {
            var vanBangs = await _locationService.GetVanBangsByTinhIdAsync(tinhId);
            foreach (var vb in vanBangs)
            {
                await _locationService.UpdateVanBangDonViCapAsync(vb.VanBangId, null);
            }
        }

        private async Task HandleEmployeesReferencingXaAsync(int xaId)
        {
            var employees = await _locationService.GetEmployeesByXaIdAsync(xaId);
            foreach (var employee in employees)
            {
                await _locationService.UpdateEmployeeLocationAsync(employee.EmployeeId, employee.TinhId, employee.HuyenId, null);
            }
        }

        private async Task HandleEmployeesReferencingHuyenAsync(int huyenId)
        {
            var employees = await _locationService.GetEmployeesByHuyenIdAsync(huyenId);
            foreach (var emp in employees)
            {
                await _locationService.UpdateEmployeeLocationAsync(emp.EmployeeId, emp.TinhId, null, null);
            }
        }

        public async Task<List<dynamic>> GetAllXaAsync(int? tinhId = null, int? huyenId = null, string? searchTerm = null)
        {
            var xas = await _locationService.GetAllXaAsync(huyenId, searchTerm);
            var result = new List<dynamic>();

            foreach (var xa in xas)
            {
                if (xa.Huyen?.TinhId == null)
                    continue;

                if (tinhId.HasValue && xa.Huyen.TinhId != tinhId.Value)
                    continue;

                var huyen = await _locationService.GetHuyenByIdAsync(xa.HuyenId ?? 0);
                var tinh = huyen?.TinhId.HasValue == true ?
                    await _locationService.GetTinhByIdAsync(huyen.TinhId.Value) :
                    null;

                result.Add(new
                {
                    xa.XaId,
                    xa.TenXa,
                    xa.HuyenId,
                    TenHuyen = huyen?.TenHuyen ?? "Không xác định",
                    TinhId = huyen?.TinhId,
                    TenTinh = tinh?.TenTinh ?? "Không xác định"
                });
            }

            return result;
        }

        public async Task<dynamic?> GetXaByIdAsync(int id)
        {
            var xa = await _locationService.GetXaByIdAsync(id);
            if (xa == null)
                return null;

            var huyen = await _locationService.GetHuyenByIdAsync(xa.HuyenId ?? 0);
            var tinh = huyen?.TinhId.HasValue == true ?
                await _locationService.GetTinhByIdAsync(huyen.TinhId.Value) :
                null;

            return new
            {
                xa.XaId,
                xa.TenXa,
                xa.HuyenId,
                TenHuyen = huyen?.TenHuyen ?? "Không xác định",
                TinhId = huyen?.TinhId,
                TenTinh = tinh?.TenTinh ?? "Không xác định"
            };
        }

        public async Task<List<DanhMucXa>> GetXasByHuyenIdAsync(int huyenId)
        {
            return await _locationService.GetXasByHuyenIdAsync(huyenId);
        }

        public async Task<DanhMucXa> CreateXaAsync(DanhMucXa xa)
        {
            ValidationResult result = await _xaValidator.ValidateAsync(xa);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors.First().ErrorMessage);
            }

            return await _locationService.CreateXaAsync(xa);
        }

        public async Task<bool> UpdateXaAsync(DanhMucXa xa)
        {
            ValidationResult result = await _xaValidator.ValidateAsync(xa);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors.First().ErrorMessage);
            }

            return await _locationService.UpdateXaAsync(xa);
        }

        public async Task<bool> DeleteXaWithDependenciesAsync(int xaId)
        {
            using var transaction = await _locationService.BeginTransactionAsync();
            try
            {
                await HandleEmployeesReferencingXaAsync(xaId);
                var result = await _locationService.DeleteXaAsync(xaId);
                if (result)
                {
                    await transaction.CommitAsync();
                    return true;
                }

                await transaction.RollbackAsync();
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Lỗi khi xóa xã: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteXaAsync(int id)
        {
            return await _locationService.DeleteXaAsync(id);
        }
    }
}