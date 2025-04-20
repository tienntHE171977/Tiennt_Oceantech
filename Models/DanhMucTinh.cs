namespace Tiennthe171977_Oceanteach.Models;

public partial class DanhMucTinh
{
    public int TinhId { get; set; }

    public string TenTinh { get; set; } = null!;

    public virtual ICollection<DanhMucHuyen> DanhMucHuyens { get; set; } = new List<DanhMucHuyen>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<VanBang> VanBangs { get; set; } = new List<VanBang>();
}