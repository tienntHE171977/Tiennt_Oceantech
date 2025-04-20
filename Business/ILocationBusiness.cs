using Tiennthe171977_Oceanteach.Models;

namespace Tiennthe171977_Oceanteach.Business
{
    public interface ILocationBusiness
    {
        #region Tỉnh Business

        Task<List<DanhMucTinh>> GetAllTinhAsync(string? searchTerm = null);

        Task<DanhMucTinh?> GetTinhByIdAsync(int id);

        Task<DanhMucTinh> CreateTinhAsync(DanhMucTinh tinh);

        Task<bool> UpdateTinhAsync(DanhMucTinh tinh);

        Task<bool> DeleteTinhAsync(int id);

        Task<bool> DeleteTinhWithDependenciesAsync(int tinhId);

        #endregion Tỉnh Business

        #region Huyện Business

        Task<List<dynamic>> GetAllHuyenAsync(int? tinhId = null, string? searchTerm = null);

        Task<dynamic?> GetHuyenByIdAsync(int id);

        Task<List<DanhMucHuyen>> GetHuyensByTinhIdAsync(int tinhId);

        Task<DanhMucHuyen> CreateHuyenAsync(DanhMucHuyen huyen);

        Task<bool> UpdateHuyenAsync(DanhMucHuyen huyen);

        Task<bool> DeleteHuyenAsync(int id);

        Task<bool> DeleteHuyenWithDependenciesAsync(int huyenId);

        #endregion Huyện Business

        #region Xã Business

        Task<List<dynamic>> GetAllXaAsync(int? tinhId = null, int? huyenId = null, string? searchTerm = null);

        Task<dynamic?> GetXaByIdAsync(int id);

        Task<List<DanhMucXa>> GetXasByHuyenIdAsync(int huyenId);

        Task<DanhMucXa> CreateXaAsync(DanhMucXa xa);

        Task<bool> UpdateXaAsync(DanhMucXa xa);

        Task<bool> DeleteXaAsync(int id);

        Task<bool> DeleteXaWithDependenciesAsync(int xaId);

        #endregion Xã Business
    }
}