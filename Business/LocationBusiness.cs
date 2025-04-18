using Tiennthe171977_Oceanteach.Models;
using Tiennthe171977_Oceanteach.Service;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace Tiennthe171977_Oceanteach.Business
{
    public class LocationBusiness : ILocationBusiness
    {
        private readonly ILocationService _locationService;

        public LocationBusiness(ILocationService locationService)
        {
            _locationService = locationService;
        }

        #region Tỉnh Business Logic

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
            // Validate tên tỉnh không trùng lặp
            var existingTinh = await _locationService.GetTinhByNameAsync(tinh.TenTinh);
            if (existingTinh != null)
            {
                throw new Exception($"Tỉnh '{tinh.TenTinh}' đã tồn tại trong hệ thống");
            }

            return await _locationService.CreateTinhAsync(tinh);
        }

        public async Task<bool> UpdateTinhAsync(DanhMucTinh tinh)
        {
            // Kiểm tra trùng tên tỉnh (ngoại trừ chính nó)
            var existingTinh = await _locationService.GetTinhByNameExceptIdAsync(tinh.TenTinh, tinh.TinhId);
            if (existingTinh != null)
            {
                throw new Exception($"Tỉnh '{tinh.TenTinh}' đã tồn tại trong hệ thống");
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

                // Sau đó tiếp tục với logic xóa huyện và xã như trước
                var huyens = await GetHuyensByTinhIdAsync(tinhId);
                foreach (var huyen in huyens)
                {
                    await DeleteHuyenWithDependenciesAsync(huyen.HuyenId);
                    

                }

                // Cuối cùng xóa tỉnh
                var result = await _locationService.DeleteTinhAsync(tinhId);
                if (result)
                {
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Lỗi khi xóa huyện: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        
        private async Task HandleEmployeesReferencingTinhAsync(int tinhId)
        {
            var employees = await _locationService.GetEmployeesByTinhIdAsync(tinhId);

            foreach (var employee in employees)
            {
                await _locationService.UpdateEmployeeLocationAsync(employee.EmployeeId, null,null,null);
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

        #endregion

        #region Huyện Business Logic

        public async Task<List<dynamic>> GetAllHuyenAsync(int? tinhId = null, string? searchTerm = null)
        {
            var huyens = await _locationService.GetAllHuyenAsync(tinhId, searchTerm);
            var result = new List<dynamic>();

            foreach (var huyen in huyens)
            {
                // Lấy thông tin tỉnh
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

            // Lấy thông tin tỉnh
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
            // Validate tên huyện không trùng lặp trong cùng tỉnh
            var existingHuyen = await _locationService.GetHuyenByNameAndTinhIdAsync(huyen.TenHuyen, huyen.TinhId ?? 0);
            if (existingHuyen != null)
            {
                throw new Exception($"Huyện '{huyen.TenHuyen}' đã tồn tại trong tỉnh này");
            }

            return await _locationService.CreateHuyenAsync(huyen);
        }

        public async Task<bool> UpdateHuyenAsync(DanhMucHuyen huyen)
        {
            // Kiểm tra trùng tên huyện trong cùng tỉnh (ngoại trừ chính nó)
            var existingHuyen = await _locationService.GetHuyenByNameAndTinhIdExceptIdAsync(
                huyen.TenHuyen, huyen.TinhId ?? 0, huyen.HuyenId);

            if (existingHuyen != null)
            {
                throw new Exception($"Huyện '{huyen.TenHuyen}' đã tồn tại trong tỉnh này");
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
                // Cập nhật lại thông tin xaID của các nhân viên liên quan
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
        #endregion

        #region Xã Business Logic

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

                // Lấy thông tin huyện
                var huyen = await _locationService.GetHuyenByIdAsync(xa.HuyenId ?? 0);

                // Lấy thông tin tỉnh
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

            // Lấy thông tin huyện
            var huyen = await _locationService.GetHuyenByIdAsync(xa.HuyenId ?? 0);

            // Lấy thông tin tỉnh
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
            // Validate tên xã không trùng lặp trong cùng huyện
            var existingXa = await _locationService.GetXaByNameAndHuyenIdAsync(xa.TenXa, xa.HuyenId ?? 0);
            if (existingXa != null)
            {
                throw new Exception($"Xã '{xa.TenXa}' đã tồn tại trong huyện này");
            }

            return await _locationService.CreateXaAsync(xa);
        }

        public async Task<bool> UpdateXaAsync(DanhMucXa xa)
        {
            // Kiểm tra trùng tên xã trong cùng huyện (ngoại trừ chính nó)
            var existingXa = await _locationService.GetXaByNameAndHuyenIdExceptIdAsync(
                xa.TenXa, xa.HuyenId ?? 0, xa.XaId);

            if (existingXa != null)
            {
                throw new Exception($"Xã '{xa.TenXa}' đã tồn tại trong huyện này");
            }

            return await _locationService.UpdateXaAsync(xa);
        }
        public async Task<bool> DeleteXaWithDependenciesAsync(int xaId)
        {
            await using var transaction = await _locationService.BeginTransactionAsync();
            try
            {
                // 1. Gỡ liên kết với Employee
                await HandleEmployeesReferencingXaAsync(xaId);

                // 2. Xóa xã
                var result = await _locationService.DeleteXaAsync(xaId);
                if (result)
                {
                    // Nếu xóa xã thành công, commit transaction
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    // Nếu không thể xóa xã, rollback transaction
                    if (transaction != null)
                        await transaction.RollbackAsync();
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Xử lý exception, rollback transaction
                if (transaction != null)
                    await transaction.RollbackAsync();
                throw new Exception($"Lỗi khi xóa xã: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }




        public async Task<bool> DeleteXaAsync(int id)
        {
            return await _locationService.DeleteXaAsync(id);
        }

        #endregion
    }
}
