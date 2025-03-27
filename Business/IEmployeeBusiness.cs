using Tiennthe171977_Oceanteach.Models;

namespace Tiennthe171977_Oceanteach.Business
{
    public interface IEmployeeBusiness
    {
        Task<bool> CreateEmployeeAsync(Employee employee);
    }
}